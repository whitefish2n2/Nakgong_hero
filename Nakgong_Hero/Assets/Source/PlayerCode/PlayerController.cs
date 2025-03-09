using System;
using System.Collections;
using Cinemachine;
using Source.Item;
using Source.Item.ItemInteract;
using Source.MonsterCode;
using Source.UiCode;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Source.PlayerCode
{
    public class PlayerController : MonoBehaviour
    {
        public static PlayerController instance;
        private Rigidbody2D _rigid;
        private Animator _anim;
        [SerializeField] private GameObject attackBox;

        [Header("대검 투척/들고있는 대검")] 
        [SerializeField] private GameObject sword;
        [SerializeField] private TrailRenderer trail;
        [SerializeField] private GameObject holdingSword;

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
        [SerializeField] private bool _isReadyNakgong;
        public bool isThrowing;
        public bool isGetHooking;
        [HideInInspector] public string attackMode;
        [HideInInspector] public Vector3 playerPos;
        [HideInInspector] public Quaternion playerRotate;
        
    
        //좌클릭으로 훅 진입함?
        private bool _goNakgong;
    
    
        [Header("상태관리")]
        public bool isStop;
        public bool doNotAttack;
        public bool isStun;
        public bool youCantHurtMe;
        [SerializeField] private bool goingRight;
        [SerializeField] private bool sHold;
        [SerializeField] private bool onGround;

        //이전 프레임에 보고 있는 ITEM OBJECT
        private Collider2D _watchingItemTemp;
        
        //animation cashing
        //bool
        private static readonly int MoveSpeed = Animator.StringToHash("MoveSpeed");
        private static readonly int LeftMoving = Animator.StringToHash("LeftMoving");
        private static readonly int RightMoving = Animator.StringToHash("RightMoving");
        //trigger
        private static readonly int Nakgong = Animator.StringToHash("Nakgong");
        private static readonly int NakgongEnd = Animator.StringToHash("NakgongEnd");
        private static readonly int IsHoldingDeager = Animator.StringToHash("is Holding Deager");
        private static readonly int HoldRefresh = Animator.StringToHash("HoldRefresh");

        private void Awake()
        {
            instance = this;
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
            HpUiManager.instance.UpdateHpBar();
            ChangeDeagerMode(SwordMode.Hold);
        }

        private void Update()
        {
            onGround = PlayerCollider.IsOnGround;
            if (!isStop)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    StartCoroutine(Jump());
                }

                if (!isStun)
                {
                    if (Input.GetKey(KeyCode.LeftShift) && !_isReadyNakgong)
                    {
                        InvManager.instance.speed = InvManager.instance.startSpeed + InvManager.instance.shiftSpeedPlus;
                        _anim.SetFloat(MoveSpeed, 2f);
                    }
                    else
                    {
                        if (!_isReadyNakgong)
                        {
                            InvManager.instance.speed = InvManager.instance.startSpeed;
                            _anim.SetFloat(MoveSpeed, 1f);
                        }
                    }

                    if (Input.GetKeyDown(KeyCode.LeftShift) && !_isReadyNakgong)
                    {
                        if (true)
                        {
                            _rigid.AddForce((goingRight ? Vector3.right : Vector3.left));
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
                    if (Input.GetKey(KeyCode.S) && _horizontal == 0f && onGround)
                    {
                        sHold = true;
                    }
                    else
                    {
                        sHold = false;
                    }
                    if (!doNotAttack)
                    {
                        //좌클릭-낙공 우클릭-던지기
                        if (Input.GetMouseButtonDown(0))
                        {
                            if (!_isNakGonging  && isThrowing && !isGetHooking)
                            {
                                isGetHooking = true;
                                _goNakgong = true;
                                GetHooking();
                            }
                            else if (!_isNakGonging && _isReadyNakgong&& !isGetHooking && !isThrowing && !onGround && !_deager._isThrowing)
                            {
                                NakGong();
                            }
                            else if (sHold)
                            {
                                EarthCrushing();
                            }
                        }

                        if (Input.GetMouseButtonDown(1))
                        {
                            if (!_isNakGonging && !isThrowing && !isGetHooking && !_deager._isThrowing)
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
            }
            if (_horizontal == 0)
            {
                _anim.SetBool(RightMoving, false);
                _anim.SetBool(LeftMoving, false);
            }
            if (onGround && movingFloor)
            {
                if (_mfDist == new Vector2(12123, 12123) || _horizontalTemp != 0 || _isJumping)
                {
                    Vector2 position = movingFloor.transform.position;
                    _mfDist = new Vector2(transform.position.x - position.x,
                        transform.position.y - position.y);
                    transform.position = (Vector2)movingFloor.transform.position + _mfDist;
                }
                else
                    transform.position = (Vector2)movingFloor.transform.position + _mfDist;
            }
            else
            {
                _mfDist = new Vector2(12123, 12123);
            }
            _horizontalTemp = _horizontal;
            if (onGround)
                throwOnAirCount = _throwOnAirCountTemp;
            //아이템 인터렉트 코드
            RaycastHit2D interactRay = Physics2D.Raycast(new Vector2(playerPos.x, playerPos.y - 1f),
                (goingRight ? Vector3.right:Vector3.left), 2, LayerMask.GetMask("Items"));
            
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
            if(!isStun && !isStop)
                _rigid.linearVelocity = new Vector2(_horizontal * InvManager.instance.speed * Time.deltaTime, _rigid.linearVelocity.y);

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
            goingRight = true;
            if (_isReadyNakgong || _isNakGonging) return;
            gameObject.transform.rotation = Quaternion.Euler(0f, -180f, 0f);
            _anim.SetBool(RightMoving, true);
            _anim.SetBool(LeftMoving, false);
        }

        //왼쪽으로 이동 애니메이션 관리
        private void GoLeft()
        {
            _horizontal = -1f;
            goingRight = false;
            if(_isReadyNakgong || _isNakGonging) return;
            gameObject.transform.rotation = Quaternion.Euler(0f, 0f, 0f);//
            _anim.SetBool(RightMoving, false);
            _anim.SetBool(LeftMoving, true);
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
                _anim.SetTrigger(Nakgong);
                //낙공 속도
                _rigid.gravityScale = InvManager.instance.startGravityScale + InvManager.instance.gravityScalePlus;
                StartCoroutine(NakgongManage());
            }
            _isReadyNakgong = false;
        }

        //대검 던지기
        private void Throwing()
        {
            if (throwOnAirCount <= 0) return;
            RefreshHoldDeager(false);
            sword.transform.position = holdingSword.transform.position;
            sword.transform.rotation = holdingSword.transform.rotation;
            ChangeDeagerMode(SwordMode.Throw);
            sword.GetComponent<SpriteRenderer>().color = Color.white;
            throwOnAirCount--;
            isThrowing = true;
            Vector3 mousePos = _camera!.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,
                Input.mousePosition.y, 0f));
            _deager.ThrowAt_withThrowRange(mousePos, chainLength);
        }
        //대검으로 이동
        private void GetHooking()
        {
            _deager.isCrashWithWall = false;
            _deager.dontCheckWithWall = true;
            StartCoroutine(GetHook());
        }

        [SerializeField] private Vector2 earthCrushingSize;
        private bool _isCanUseEarthCrushing = true;
        private Camera _camera;

        private void EarthCrushing()
        {
            if (!_isCanUseEarthCrushing || doNotAttack || _deager._isThrowing|| _isReadyNakgong || !onGround) return;
            
            StartCoroutine(MainCameraShakeDiscourage(5, 0, 0.1f));
            GameUtil.instance.CoolBool(1, v=> _isCanUseEarthCrushing = v);
            RaycastHit2D[] rayR = Physics2D.BoxCastAll(transform.position, earthCrushingSize, 0f, Vector2.right, earthCrushingSize.x / 2,
                LayerMask.GetMask("Monster"));;
            RaycastHit2D[] rayL = Physics2D.BoxCastAll(transform.position, earthCrushingSize, 0f, Vector2.left, earthCrushingSize.x / 2,
                LayerMask.GetMask("Monster"));
            foreach(var o in rayL)
            {
                if(o.transform.CompareTag("DefaultMonster"))
                    o.collider.gameObject.GetComponent<DefaultMonster>().GotAirbornAttack("대지분쇄", InvManager.instance.GetAttackPower()*1.1f, InvManager.instance.stansBreak, 500);
            }
            foreach(var o in rayR)
            {
                if(o.transform.CompareTag("DefaultMonster"))
                    o.collider.gameObject.GetComponent<DefaultMonster>().GotAirbornAttack("대지분쇄", InvManager.instance.GetAttackPower()*1.1f, InvManager.instance.stansBreak, 500);
            }
        }

        //공격받았을때
        public void GotAttack(float damage, bool absolute = false, float invincibleTime = 0, float stunTime = 0)
        {
            if (youCantHurtMe)  if(!absolute) return;
            InvManager.instance.hp -= damage;
            HpUiManager.instance.UpdateHpBar();
            if(invincibleTime > 0) Invincibility(invincibleTime);
            if(stunTime>0)GameUtil.instance.CoolBool(stunTime, v=> isStun = v, false);
        }

        //대검쪽으로 이동 코루틴
        private IEnumerator GetHook()
        {
            float gravityTemp = _rigid.gravityScale;
            _rigid.linearVelocity = Vector2.zero;
            _rigid.gravityScale = 0f;
            isStun = true;
            isThrowing = false;
            _horizontal = 0f;
            _rigid.linearVelocity = Vector2.zero;
            while (isGetHooking && Vector2.Distance(sword.transform.position,transform.position) > 0.9f)
            {
                _rigid.linearVelocity = (sword.transform.position - playerPos) * getHookSpeed;
                yield return null;
                /*transform.position += (sword.transform.position - playerPos).normalized * (Time.deltaTime * getHookSpeed);
                if (_playerCollider.IsColliding)
                {
                    _goNakgong = false;
                    break;
                }
                yield return null;*/
            }
            isGetHooking = false;
            isStun = false;
            _rigid.gravityScale = gravityTemp;
            if (_isReadyNakgong)
            {
                //낙공 준비 모션으로 anim 수정
            }
            if (_goNakgong)
            {
                if (Vector2.Distance(sword.transform.position, transform.position) < 0.9f)
                {
                    _deager.InstanceTurnBack();
                }
                else _deager.StartCoroutine("TurnBack");
                while(_deager._isThrowing) yield return null;
                _isReadyNakgong = true;
                _rigid.linearVelocity = new Vector2(_rigid.linearVelocity.x,InvManager.instance.jumpPower);
                yield return new WaitForSeconds(0.2f);
                NakGong();
                isGetHooking = false;
                _goNakgong = false;
                yield break;
            }
            isGetHooking = false;
            _goNakgong = false;
            if(Vector2.Distance(sword.transform.position,transform.position) < 0.9f)
            {
                _deager.InstanceTurnBack();
                Debug.Log("빠른;");
            }
            else _deager.StartCoroutine("TurnBack");
                
        }

        //점프(2단점프 포함)
        private IEnumerator Jump()
        {
            if (isThrowing || isGetHooking) yield break;
            if (onGround)
            {
                _isJumping = true;
                _rigid.linearVelocity = new Vector2(_rigid.linearVelocity.x,InvManager.instance.jumpPower);
                yield return new WaitForSeconds(0.1f);
            }
            else if (!doNotAttack)
            {
                _isReadyNakgong = true;
                _isJumping = false;
                _rigid.linearVelocity = new Vector2(_rigid.linearVelocity.x,InvManager.instance.jumpPower);
                StartCoroutine(NakgongManage());
                yield break;
            }
            while (!onGround)
            {
                yield return null;
            }
            _isJumping = false;
        }
        //낙공할 때 바닥에 닿을 때까지 속도가 느려지고 낙공 가중치 만드는 청년(낙공 판정 서포터)
        private IEnumerator NakgongManage()
        {
            //bool tempforbug = _playerCollider.isOnGround;
            yield return new WaitForSeconds(0.04f);
            while (!onGround)
            {
                if (_isReadyNakgong && InvManager.instance.speed > 0f)
                {
                    InvManager.instance.speed -= 300f * Time.deltaTime;
                }

                if (_isNakGonging)
                {
                
                    if (attackMode == "Default")
                    {
                        InvManager.instance.airBonePower += 300f * Time.deltaTime;
                    }
                }
                yield return null;
            }
            if (_isReadyNakgong)
            {
                _isReadyNakgong = false;
                InvManager.instance.speed = InvManager.instance.startSpeed;
            }
            if (_isNakGonging)
            {
                _isNakGonging = false;
                ChangeNakgongCam();
                StartCoroutine(MainCameraShakeDiscourage(5, 0, 0.1f));
                _anim.SetTrigger(NakgongEnd);
                _horizontal = 0;
                GameUtil.instance.CoolBool(0.5f, v => isStun = v, false);
                if (attackMode == "Default")
                {
                    InvManager.instance.airBonePower = 0f;
                    attackBox.SetActive(false);
                }
            }
            CameraDefaultMove.CameraposPlus = 0f;
            _rigid.gravityScale = InvManager.instance.startGravityScale;
            _rigid.linearVelocity = Vector2.zero;
        }

        private void Invincibility(float t)
        {
            GameUtil.instance.CoolBool(t,v=>youCantHurtMe = v, false);
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

        private void ChangeNakgongCam()
        {
            vNakgongCam.Priority = _isNakGonging ? 11 : 9;
        }

        private void RefreshHoldDeager(bool hold)
        {
            _anim.SetBool(IsHoldingDeager,hold);
            _anim.SetTrigger(HoldRefresh);
        }

        public void ChangeDeagerMode(SwordMode mode)
        {
            switch (mode)
            {
                case SwordMode.Hold:
                    holdingSword.SetActive(true);
                    sword.SetActive(false);
                    break;
                case SwordMode.Throw:
                    holdingSword.SetActive(false);
                    sword.SetActive(true);
                    break;
            }
        }
        public void SetNormal()
        {
            StopCoroutine(nameof(NakgongManage));
            StopCoroutine(nameof(Jump));
            _deager.InstanceTurnBack();
            _isNakGonging = false;
            isThrowing = false;
            isGetHooking = false;
            _goNakgong = false;
            isStun = false;
            _anim.StopPlayback();
            _anim.SetTrigger(NakgongEnd);
            ChangeNakgongCam();
            _rigid.gravityScale = 1f;
        }

        public void InstantBreakMove()
        {
            _horizontal = 0f;
            _rigid.linearVelocity = Vector2.zero;
        }

        private IEnumerator _currentXBreak;
        public void X_BreakTil(float t)
        {
            StartCoroutine(XBreakTil(t));
        }

        public void StopXBreak()
        {
            StopCoroutine(_currentXBreak);
            isStun = false;
        }
        private IEnumerator XBreakTil(float t)
        {
            float elapsedTime = 0;
            isStun = true;
            while (elapsedTime < t)
            {
                _horizontal = 0f;
                elapsedTime += Time.deltaTime;
                _rigid.linearVelocity = new Vector2(0, _rigid.linearVelocity.y);
                yield return null;
            }

            isStun = false;
        }

        public enum SwordMode
        {
            Throw,
            Hold
        }

        public void HpCheck()
        {
            if (InvManager.instance.hp <= 0)
            {
                SceneManager.LoadScene("DeadScene");
            }
        }
    }
}
