    using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Source.MonsterCode;
using Source.PlayerCode;
using Unity.Mathematics;
using Unity.Properties;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;
    private Rigidbody2D _rigid;
    private Animator _anim;
    [SerializeField] private GameObject attackBox;

    [FormerlySerializedAs("Sword")] [Header("대검 투척")] [SerializeField]
    private GameObject sword;

    private Deager _deager;

    [Header("버츄얼 카메라")] [SerializeField] private CinemachineVirtualCamera vCam;
    private CinemachineBasicMultiChannelPerlin noise;
    private CinemachineBasicMultiChannelPerlin nakgongNoise;
    [SerializeField] private CinemachineVirtualCamera vNakgongCam;

    [SerializeField] private float chainLength;
    [SerializeField] private float getHookSpeed;
    [SerializeField] int throwOnAirCount;

    private int _throwOnAirCountTemp;

    //땅에 닿았는지를 판별하는 class
    private PlayerCollider _playerCollider;

    //내가 올라간 움직이는 바닥 위치를 저장
    [HideInInspector] public GameObject movingFloor = null;
    private Vector2 _mfDist; 

    //수평이동
    private float _horizontal;
    private float _horizontalTemp;

    //점프 중/ 낙공 중 판별 bool
    private bool _isNakGonging = false;
    private bool _isJumping = false;
    private bool _isReadyNakgong = false;
    [HideInInspector] public bool isThrowing = false;
    [HideInInspector] public bool isGetHooking = false;
    [HideInInspector] public string attackMode;
    [HideInInspector] public Vector3 playerPos;
    [HideInInspector] public Quaternion playerRotate;
    private bool _goingRight;
    private bool _sHold;
    private bool _onGround;
    
    //좌클릭으로 훅 진입함?
    private bool goNakgong = false;
    
    
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
        _rigid = gameObject.GetComponent<Rigidbody2D>();
        _anim = gameObject.GetComponent<Animator>();
        _deager = sword.GetComponent<Deager>();
    }

    private void Start()
    {
        noise = vCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        nakgongNoise = vNakgongCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
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

            if (_isReadyNakgong ||  _horizontal == 0)
            {
                if (_onGround && movingFloor)
                {
                    if (_mfDist == new Vector2(12123, 12123) || _horizontalTemp != 0 || _isJumping)
                    {
                        Vector2 position = movingFloor.transform.position;
                        _mfDist = new Vector2(transform.position.x - position.x,
                            transform.position.y - position.y);
                        Debug.Log("reFresh");
                    }
                    else
                        transform.position = (Vector2)movingFloor.transform.position + _mfDist;
                    
                }
                else
                {
                    _mfDist = new Vector2(12123, 12123);
                }
                if (!_anim.GetCurrentAnimatorStateInfo(0).IsName("Default"))
                {
                    _anim.StopPlayback();
                    _anim.Play("Default");
                }
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
                        goNakgong = true;
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

            _horizontalTemp = _horizontal;
        }
        if (_onGround)
            throwOnAirCount = _throwOnAirCountTemp;
        //아이템 인터렉트 코드
        RaycastHit2D interactRay = Physics2D.Raycast(new Vector2(playerPos.x, playerPos.y - 1f),
            (_goingRight ? Vector3.right:Vector3.left), 2, LayerMask.GetMask("Items", "MovingObjects"));
        if (interactRay.collider is not null)
        {
            if (interactRay.collider.gameObject.CompareTag("CommonItem"))
            {
                ItemInteract.Instance.ChangeText("획득");
                if (_watchingItemTemp)
                {
                    _watchingItemTemp.GetComponent<CommonItemOBJ>().DisWatching();
                }

                interactRay.collider.GetComponent<CommonItemOBJ>().Watching();
                _watchingItemTemp = interactRay.collider;
            }
            else if (interactRay.collider.gameObject.CompareTag("MovingObject"))
            {
                ItemInteract.Instance.ChangeText("상호작용");
                if (_watchingItemTemp)
                {
                    _watchingItemTemp.GetComponent<CommonItemOBJ>().DisWatching();
                }

                interactRay.collider.GetComponent<CommonItemOBJ>().Watching();
                _watchingItemTemp = interactRay.collider;
            }
        }
    }

    private void FixedUpdate()
    {
        playerPos = gameObject.transform.position;
        playerRotate = gameObject.transform.rotation;
        _rigid.velocity = new Vector2(_horizontal * InvManager.Instance.speed * Time.deltaTime, _rigid.velocity.y);

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

    public void toAttacker()
    {
        doNotAttack = true;
    }

    public void toNotAttacker()
    {
        doNotAttack = false;
    }
    //오른쪽으로 이동 애니메이션 관리
    private void GoRight()
    {
        _horizontal = 1f;
        gameObject.transform.rotation = new Quaternion(0f, 180f, 0f, 0f);
        _goingRight = true;
        if (_isReadyNakgong) return;
        if (_anim.GetCurrentAnimatorStateInfo(0).IsName("LeftMove") == false);
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
            _rigid.gravityScale = InvManager.Instance.startGravityScale + InvManager.Instance.GravityScalePlus;
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
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,
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
    private void EarthCrushing()
    {
        if (_isCanUseEarthCrushing)
        {
            StartCoroutine(CoolBool(1, (v=> _isCanUseEarthCrushing = v)));
            RaycastHit2D[] rayR = Physics2D.BoxCastAll(transform.position, new Vector2(0.01f, earthCrushingSize.y), 0,
                Vector2.right, earthCrushingSize.x, LayerMask.GetMask("Monster"));
            RaycastHit2D[] rayL = Physics2D.BoxCastAll(transform.position, new Vector2(0.01f, earthCrushingSize.y), 0,
                Vector2.left, earthCrushingSize.x, LayerMask.GetMask("Monster"));
            foreach(var o in rayL)
            {
                if(o.transform.CompareTag("DefaultMonster"))
                    o.collider.gameObject.GetComponent<DefaultMonster>().GotAttack("대지분쇄", InvManager.Instance.AttackPower*1.1f, InvManager.Instance.stans, 500);
            }
            foreach(var o in rayR)
            {
                if(o.transform.CompareTag("DefaultMonster"))
                    o.collider.gameObject.GetComponent<DefaultMonster>().GotAttack("대지분쇄", InvManager.Instance.AttackPower*1.1f, InvManager.Instance.stans, 500);
            }
        }
    }

    //공격받았을때
    public void GotAttack(float damage, float stunTime)
    {
        InvManager.Instance.HP -= damage;
        Debug.Log(InvManager.Instance.HP);
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
                goNakgong = false;
                break;
            }
            yield return null;
        }
        
        _rigid.gravityScale = gravityTemp;
        if (goNakgong)
        {
            _isReadyNakgong = true;
            _rigid.velocity = new Vector2(_rigid.velocity.x,InvManager.Instance.jumpPower/2);
            _deager.InstanceTurnBack();
            NakGong();
            isGetHooking = false;
            goNakgong = false;
            yield break;
        }
        _deager.StartCoroutine("TurnBack");
        isGetHooking = false;
        goNakgong = false;
    }

    //점프(2단점프 포함)
    IEnumerator Jump()
    {
        _isJumping = true;
        _rigid.velocity = new Vector2(_rigid.velocity.x,InvManager.Instance.jumpPower);
        yield return new WaitForSeconds(0.1f);
          
        while (!_onGround)
        {
            if (Input.GetKeyDown(KeyCode.Space) && !isThrowing && !doNotAttack)
            {
                _isReadyNakgong = true;
                _isJumping = false;
                _rigid.velocity = new Vector2(_rigid.velocity.x,InvManager.Instance.jumpPower);
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
                    InvManager.Instance.AirBonePower += 300f * Time.deltaTime;
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
                InvManager.Instance.AirBonePower = 0f;
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
            noise.m_AmplitudeGain = math.lerp(start, end, time);
            yield return null;
        }
        noise.m_AmplitudeGain = end;
    }

    private IEnumerator CoolBool(float t, Action<bool> target)
    {
        target(false);
        yield return new WaitForSeconds(t);
        target(true);
    }

    private void ChangeNakgongCam()
    {
        if (_isNakGonging)
        {
            vNakgongCam.Priority = 11;
            vCam.Priority = 10;
        }
        else
        {
            vNakgongCam.Priority = 10;
            vCam.Priority = 11;
        }
    }
}
