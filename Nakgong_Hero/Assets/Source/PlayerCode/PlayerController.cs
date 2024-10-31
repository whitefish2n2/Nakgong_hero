using System;
using System.Collections;
using Cinemachine;
using Source.Item;
using Source.MonsterCode;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace Source.PlayerCode
{
    public class PlayerController : MonoBehaviour
    {
        public static PlayerController Instance;
        private Rigidbody2D _rigid;
        private Animator _anim;
        [SerializeField] private GameObject attackBox;

        [FormerlySerializedAs("Sword")] [Header("대검 투척")] [SerializeField]
        private GameObject sword;

        private Deager _deager;

        [Header("버츄얼 카메라")] 
        [SerializeField] private CinemachineBrain cinemachineBrain;
        [SerializeField] private CinemachineVirtualCamera vCam;
        private CinemachineBasicMultiChannelPerlin _noise;
        [SerializeField] private CinemachineVirtualCamera vNakgongCam;

        [SerializeField] private float chainLength;
        [SerializeField] private float getHookSpeed;
        [SerializeField] private int throwOnAirCount;

        private int _throwOnAirCountTemp;

        //땅에 닿았는지를 판별하는 class
        private PlayerCollider _playerCollider;

        //내가 올라간 움직이는 바닥 위치를 저장
        [HideInInspector] public GameObject movingFloor;
        private Vector2 _mfDist; 

        //수평이동
        private float _horizontal;
        private float _horizontalTemp;

        //점프 중/ 낙공 중 판별 bool
        private bool _isNakGonging;
        private bool _isJumping;
        private bool _isReadyNakgong;
        [HideInInspector] public bool isThrowing;
        [HideInInspector] public bool isGetHooking;
        [HideInInspector] public string attackMode;
        [HideInInspector] public Vector3 playerPos;
        [HideInInspector] public Quaternion playerRotate;
        private bool _goingRight;
        private bool _sHold;
        private bool _onGround;
    
        //좌클릭으로 훅 진입함?
        private bool _goNakgong;
    
    
        [Header("상태관리")]
        [SerializeField] private bool isStop;
        [SerializeField] private bool doNotAttack;

        //이전 프레임에 보고 있는 ITEM OBJECT
        private Collider2D _watchingItemTemp;
        private static readonly int MoveSpeed = Animator.StringToHash("MoveSpeed");

        private void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            DontDestroyOnLoad(cinemachineBrain);
            DontDestroyOnLoad(vCam);
            DontDestroyOnLoad(vNakgongCam);
            _rigid = gameObject.GetComponent<Rigidbody2D>();
            _anim = gameObject.GetComponent<Animator>();
            _deager = sword.GetComponent<Deager>();
        }

        private void Start()
        {
            _camera = Camera.main;
            _noise = vCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            _throwOnAirCountTemp = throwOnAirCount;
            playerPos = transform.position;
            attackMode = "Default";
            attackBox.SetActive(false);
            _playerCollider = GetComponent<PlayerCollider>();
        }

        private void Update()
        {
            _onGround = PlayerCollider.IsOnGround;
            if (!isStop)
            {
                if (Input.GetKeyDown(KeyCode.Space) && _onGround)
                {
                    StartCoroutine(Jump());
                }

                if (Input.GetKey(KeyCode.LeftShift) && !_isReadyNakgong)
                {
                    InvManager.Instance.speed = InvManager.Instance.startSpeed + InvManager.Instance.shiftSpeedPlus;
                    _anim.SetFloat(MoveSpeed, 2f);
                }
                else
                {
                    if (!_isReadyNakgong)
                    {
                        InvManager.Instance.speed = InvManager.Instance.startSpeed;
                        _anim.SetFloat(MoveSpeed, 1f);
                    }
                }

                if (Input.GetKeyDown(KeyCode.LeftShift) && !_isReadyNakgong)
                {
                    if (true)
                    {
                        _rigid.AddForce((_goingRight ? Vector3.right : Vector3.left));
                    }
                }

                if (Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
                {
                    GoLeft();
                }

                if (Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A))
                {
                    GoRight();
                }

                if (!Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A))
                {
                    _horizontal = 0f;
                }
                if (Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.A))
                {
                    _horizontal = 0f;
                }
                if (Input.GetKey(KeyCode.S) && _horizontal == 0f && _onGround)
                {
                    _sHold = true;
                }
                else
                {
                    _sHold = false;
                }
                if (!doNotAttack)
                {
                    //좌클릭-낙공 우클릭-던지기
                    if (Input.GetMouseButtonDown(0))
                    {
                        if (_isReadyNakgong)
                        {
                            NakGong();
                        }
                        else if (!_isNakGonging && !isGetHooking && isThrowing)
                        {
                            isGetHooking = true;
                            _goNakgong = true;
                            GetHooking();
                        }
                        else if (_sHold)
                        {
                            EarthCrushing();
                        }
                    }

                    if (Input.GetMouseButtonDown(1))
                    {
                        if (!_isNakGonging && !isThrowing && !_isReadyNakgong)
                        {
                            Throwing();
                        }
                        else if (isThrowing && !isGetHooking)
                        {
                            isGetHooking = true;
                            GetHooking();
                        }
                    }
                }
            }
            if (_isReadyNakgong ||  _horizontal == 0)
            {
                if (!_anim.GetCurrentAnimatorStateInfo(0).IsName("Default"))
                {
                    _anim.StopPlayback();
                    _anim.Play("Default");
                }
            }
            if (_onGround && movingFloor)
            {
                if (_mfDist == new Vector2(12123, 12123) || _horizontalTemp != 0 || _isJumping)
                {
                    Vector2 position = movingFloor.transform.position;
                    _mfDist = new Vector2(transform.position.x - position.x,
                        transform.position.y - position.y);
                    transform.position = (Vector2)movingFloor.transform.position + _mfDist;
                    Debug.Log("reFresh");
                }
                else
                    transform.position = (Vector2)movingFloor.transform.position + _mfDist;
                    
            }
            else
            {
                _mfDist = new Vector2(12123, 12123);
            }
            _horizontalTemp = _horizontal;
            if (_onGround)
                throwOnAirCount = _throwOnAirCountTemp;
            //아이템 인터렉트 코드
            RaycastHit2D interactRay = Physics2D.Raycast(new Vector2(playerPos.x, playerPos.y - 1f),
                (_goingRight ? Vector3.right:Vector3.left), 2, LayerMask.GetMask("Items", "MovingObjects"));
            
            if (interactRay.collider?.gameObject.CompareTag("CommonItem") ?? false)
            {
                if (_watchingItemTemp != interactRay.collider)
                {
                    if (_watchingItemTemp)
                    {
                        _watchingItemTemp.GetComponent<CommonItemOBJ>().DisWatching();
                    }
                    if (interactRay.collider is not null)
                    {
                        var objCode = interactRay.collider.GetComponent<CommonItemOBJ>();
                        objCode.Watching();
                        ItemInteract.Instance.ChangeText(objCode.itemInfo.interactionDescription);
                    }
                }
                if (Input.GetKeyDown(KeyCode.F))
                {
                    if (interactRay.collider) interactRay.collider.GetComponent<CommonItemOBJ>().Get();
                }
            }
            else
            {
                if (_watchingItemTemp)
                {
                    _watchingItemTemp.GetComponent<CommonItemOBJ>().DisWatching();
                }
            }
            _watchingItemTemp = interactRay.collider;
        }

        private void FixedUpdate()
        {
            playerPos = gameObject.transform.position;
            playerRotate = gameObject.transform.rotation;
            _rigid.linearVelocity = new Vector2(_horizontal * InvManager.Instance.speed * Time.deltaTime, _rigid.linearVelocity.y);

            //debug section
        }

        public void Stop()
        {
            isStop = true;
            _horizontal = 0f;
        }

        public void DisStop()
        {
            isStop = false;
        }

        public void BeAttackAble()
        {
            doNotAttack = false;
        }
        public void BeAttackDisable()
        {
            doNotAttack = true;
        }
        //오른쪽으로 이동 애니메이션 관리
        private void GoRight()
        {
            _horizontal = 1f;
            gameObject.transform.rotation = new Quaternion(0f, 180f, 0f, 0f);
            _goingRight = true;
            if (_isReadyNakgong) return;
            if (_anim.GetCurrentAnimatorStateInfo(0).IsName("LeftMove") == false)
            {
                _anim.Play("LeftMove");
            }
        }

        //왼쪽으로 이동 애니메이션 관리
        private void GoLeft()
        {
            _horizontal = -1f;
            gameObject.transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
            _goingRight = false;
            if(_isReadyNakgong) return;
            if (_anim.GetCurrentAnimatorStateInfo(0).IsName("LeftMove") == false)
            {
                _anim.Play("LeftMove");
            }
        }

        //낙하공격 고고
        private void NakGong()
        {
            if (!_isNakGonging && !isGetHooking && !isThrowing && _isReadyNakgong && !doNotAttack)
            {
                CameraDefaultMove.CameraposPlus = -2f;
                attackBox.SetActive(true);
                _isNakGonging = true;
                ChangeNakgongCam();
                //낙공 속도
                _rigid.gravityScale = InvManager.Instance.startGravityScale + InvManager.Instance.gravityScalePlus;
                StartCoroutine(GroundedChecker());
            }
            _isReadyNakgong = false;
        }

        //대검 던지기
        private void Throwing()
        {
            if (throwOnAirCount > 0)
            {
                sword.GetComponent<SpriteRenderer>().color = Color.white;
                throwOnAirCount--;
                isThrowing = true;
                Vector3 mousePos = _camera!.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,
                    Input.mousePosition.y, 0f));
                _deager.ThrowAt_withThrowRange(mousePos, chainLength);
            }
        }
        //대검으로 이동
        private void GetHooking()
        {
            Deager.isCrashWithWall = false;
            GroundDeagerCheck.dontCheck = true;
            StartCoroutine(GetHook());
        }

        [SerializeField] private Vector2 earthCrushingSize;
        private bool _isCanUseEarthCrushing = true;
        private Camera _camera;

        private void EarthCrushing()
        {
            if (!_isCanUseEarthCrushing || doNotAttack) return;
            
            StartCoroutine(MainCameraShakeDiscourage(5, 0, 0.1f));
            StartCoroutine(CoolBool(1, (v=> _isCanUseEarthCrushing = v)));
            RaycastHit2D[] rayR = { }, rayL = { };
            Physics2D.BoxCastNonAlloc(transform.position, new Vector2(0.01f, earthCrushingSize.y), 0, Vector2.right, rayR, earthCrushingSize.x, LayerMask.GetMask("Monster"));
            Physics2D.BoxCastNonAlloc(transform.position, new Vector2(0.01f, earthCrushingSize.y), 0, Vector2.left, rayL, earthCrushingSize.x, LayerMask.GetMask("Monster"));
            foreach(var o in rayL)
            {
                if(o.transform.CompareTag("DefaultMonster"))
                    o.collider.gameObject.GetComponent<DefaultMonster>().GotAttack("대지분쇄", InvManager.Instance.attackPower*1.1f, InvManager.Instance.stans, 500);
            }
            foreach(var o in rayR)
            {
                if(o.transform.CompareTag("DefaultMonster"))
                    o.collider.gameObject.GetComponent<DefaultMonster>().GotAttack("대지분쇄", InvManager.Instance.attackPower*1.1f, InvManager.Instance.stans, 500);
            }
        }

        //공격받았을때
        public static void GotAttack(float damage, float stunTime)
        {
            InvManager.Instance.hp -= damage;
            Debug.Log(InvManager.Instance.hp);
            //스턴 조@지는 코루틴으로
        }

        //대검쪽으로 이동 코루틴
        IEnumerator GetHook()
        {
            Vector3 startPos = playerPos;
            Vector3 getHere = sword.transform.position;
            float elapsedTime = 0f;
            float gravityTemp = _rigid.gravityScale;
            _rigid.gravityScale = 0f;
            while (elapsedTime < getHookSpeed && isGetHooking)
            {
                gameObject.transform.position = Vector3.Lerp(startPos, getHere, elapsedTime / getHookSpeed);
                elapsedTime += Time.deltaTime;
                if (_playerCollider.IsColliding)
                {
                    isGetHooking = false;
                    _goNakgong = false;
                    break;
                }
                yield return null;
            }
        
            _rigid.gravityScale = gravityTemp;
            if (_goNakgong)
            {
                _isReadyNakgong = true;
                _rigid.linearVelocity = new Vector2(_rigid.linearVelocity.x,InvManager.Instance.jumpPower/2);
                _deager.InstanceTurnBack();
                NakGong();
                isGetHooking = false;
                _goNakgong = false;
                yield break;
            }
            _deager.StartCoroutine("TurnBack");
            isGetHooking = false;
            _goNakgong = false;
        }

        //점프(2단점프 포함)
        IEnumerator Jump()
        {
            _isJumping = true;
            _rigid.linearVelocity = new Vector2(_rigid.linearVelocity.x,InvManager.Instance.jumpPower);
            yield return new WaitForSeconds(0.1f);
          
            while (!_onGround)
            {
                if (Input.GetKeyDown(KeyCode.Space) && !isThrowing && !doNotAttack)
                {
                    _isReadyNakgong = true;
                    _isJumping = false;
                    _rigid.linearVelocity = new Vector2(_rigid.linearVelocity.x,InvManager.Instance.jumpPower);
                    StartCoroutine(GroundedChecker());
                    throwOnAirCount = _throwOnAirCountTemp;
                    yield break;
                }
                yield return null;
            }

            _isJumping = false;
            throwOnAirCount = _throwOnAirCountTemp;
        }
        //낙공할 때 바닥에 닿을 때까지 속도가 느려지고 낙공 가중치 만드는 청년(낙공 판정 서포터)
        private IEnumerator GroundedChecker()
        {
            //bool tempforbug = _playerCollider.isOnGround;
            yield return new WaitForSeconds(0.04f);
            while (!_onGround)
            {
                if (_isReadyNakgong && InvManager.Instance.speed > 0f)
                {
                    InvManager.Instance.speed -= 300f * Time.deltaTime;
                }

                if (_isNakGonging)
                {
                
                    if (attackMode == "Default")
                    {
                        InvManager.Instance.airBonePower += 300f * Time.deltaTime;
                    }
                }
                yield return null;
            }
            if (_isReadyNakgong)
            {
                _isReadyNakgong = false;
                InvManager.Instance.speed = InvManager.Instance.startSpeed;
            }
            if (_isNakGonging)
            {
                _isNakGonging = false;
                ChangeNakgongCam();
                StartCoroutine(MainCameraShakeDiscourage(5, 0, 0.1f));
                if (attackMode == "Default")
                {
                    InvManager.Instance.airBonePower = 0f;
                    attackBox.SetActive(false);
                }
            }
            CameraDefaultMove.CameraposPlus = 0f;
            _rigid.gravityScale = InvManager.Instance.startGravityScale;
        }

        private IEnumerator MainCameraShakeDiscourage(float start, float end, float time)
        {
            float elapsedTime = 0;
            while (elapsedTime < time)
            {
                elapsedTime += Time.deltaTime;
                _noise.m_AmplitudeGain = math.lerp(start, end, time);
                yield return null;
            }
            _noise.m_AmplitudeGain = end;
        }

        private IEnumerator CoolBool(float t, Action<bool> target)
        {
            target(false);
            yield return new WaitForSeconds(t);
            target(true);
        }

        private void ChangeNakgongCam()
        {
            vNakgongCam.Priority = _isNakGonging ? 11 : 9;
        }
    }
}
