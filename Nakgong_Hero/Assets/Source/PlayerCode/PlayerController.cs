    using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.Mathematics;
using Unity.Properties;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D _rigid;
    private Animator _anim;
    [SerializeField] private GameObject attackBox;

    [FormerlySerializedAs("Sword")] [Header("대검 투척")] [SerializeField]
    private GameObject sword;

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
    public static GameObject movingFloor;
    private Vector2 _mfDist; 

    //수평이동
    public static float Horizontal;
    private float _horizontalTemp;

    //점프 중/ 낙공 중 판별 bool
    private bool _isNakGonging = false;
    private bool _isJumping = false;
    private bool _isReadyNakgong = false;
    public static bool IsThrowing = false;
    public static bool IsGetHooking = false;
    public static string AttackMode;
    public static Vector3 PlayerPos;
    public static Quaternion PlayerRotate;
    private bool _goingRight = true;

    private static bool _isStop;
    public static bool DoNotAttack;

    //이전 프레임에 보고 있는 ITEM OBJECT
    private Collider2D _watchingItemTemp;
    private static readonly int MoveSpeed = Animator.StringToHash("MoveSpeed");

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(sword);
        _rigid = gameObject.GetComponent<Rigidbody2D>();
        _anim = gameObject.GetComponent<Animator>();
    }

    private void Start()
    {
        noise = vCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        nakgongNoise = vNakgongCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        _throwOnAirCountTemp = throwOnAirCount;
        PlayerPos = transform.position;
        AttackMode = "Default";
        attackBox.SetActive(false);
        _playerCollider = GetComponent<PlayerCollider>();

    }

    private void Update()
    {
        //이동/애니메이션 재생
        if (!_isStop)
        {
            if (Input.GetKeyDown(KeyCode.Space) && _playerCollider.isOnGround)
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
                    _rigid.AddForce((_goingRight ? Vector3.left : Vector3.right));
                }
            }

            if (Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
            {
                GoRight();
            }

            if (Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A))
            {
                GoLeft();
            }

            if (!Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A))
            {
                Horizontal = 0f;
            }
            if (Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.A))
            {
                Horizontal = 0f;
            }

            if (_isReadyNakgong ||  Horizontal == 0)
            {
                if (movingFloor is not null)
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

            if (!DoNotAttack)
            {
                //좌클릭-낙공 우클릭-던지기
                if (Input.GetMouseButtonDown(0))
                {
                    if (_isReadyNakgong)
                    {
                        NakGong();
                    }
                    else if (!_isNakGonging && !IsGetHooking && IsThrowing)
                    {
                        IsGetHooking = true;
                        GetHooking();
                    }
                }

                if (Input.GetMouseButtonDown(1))
                {
                    if (!_isNakGonging && !IsThrowing && !_isReadyNakgong)
                    {
                        Throwing();
                    }
                    else if (IsThrowing && !IsGetHooking)
                    {
                        IsGetHooking = true;
                        GetHooking();
                    }
                }
            }

            _horizontalTemp = Horizontal;
        }

        //아이템 인터렉트 코드
        RaycastHit2D interactRay = Physics2D.Raycast(new Vector2(PlayerPos.x, PlayerPos.y - 1f),
            (_goingRight ? Vector3.left : Vector3.right), 2, LayerMask.GetMask("Items", "MovingObjects"));
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
        PlayerPos = gameObject.transform.position;
        PlayerRotate = gameObject.transform.rotation;
        _rigid.velocity = new Vector2(Horizontal * InvManager.Instance.speed * Time.deltaTime, _rigid.velocity.y);
        if (_playerCollider.isOnGround)
            throwOnAirCount = _throwOnAirCountTemp;

        //debug section
    }

    public static void Stop()
    {
        _isStop = true;
        Horizontal = 0f;
    }

    public static void DisStop()
    {
        _isStop = false;
    }

    private void GoLeft()
    {
        Horizontal = 1f;
        gameObject.transform.rotation = new Quaternion(0f, 180f, 0f, 0f);
        _goingRight = false;
        if (!_isReadyNakgong && _anim.GetCurrentAnimatorStateInfo(0).IsName("LeftMove") == false);
        {
            _anim.Play("LeftMove");
        }
    }

    private void GoRight()
    {
        Horizontal = -1f;
        gameObject.transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
        _goingRight = true;
        if (!_isReadyNakgong && _anim.GetCurrentAnimatorStateInfo(0).IsName("LeftMove") == false)
        {
            _anim.Play("LeftMove");
        }
    }

    private void NakGong()
    {
        if (!_isNakGonging && !IsGetHooking && !IsThrowing && _isReadyNakgong)
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

    private void Throwing()
    {
        if (throwOnAirCount > 0)
        {
            sword.GetComponent<SpriteRenderer>().color = Color.white;
            throwOnAirCount--;
            IsThrowing = true;
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,
                Input.mousePosition.y, 0f));
            sword.GetComponent<Deager>().ThrowAt_withThrowRange(mousePos, chainLength);
        }
    }

    private void GetHooking()
    {
        Deager.isCrashWithWall = false;
        StartCoroutine(GetHook());
    }

    public static void GotAttack(float damage, float stunTime)
    {
        InvManager.Instance.HP -= damage;
        Debug.Log(InvManager.Instance.HP);
        //스턴 조@지는 코루틴으로
    }

//대검쪽으로 이동 코루틴
    IEnumerator GetHook()
    {
        Vector3 startPos = PlayerPos;
        Vector3 getHere = sword.transform.position;
        float elapsedTime = 0f;
        float gravityTemp = _rigid.gravityScale;
        bool goNakgong = false;
        _rigid.gravityScale = 0f;
        while (elapsedTime < getHookSpeed && IsGetHooking)
        {
            gameObject.transform.position = Vector3.Lerp(startPos, getHere, elapsedTime / getHookSpeed);
            elapsedTime += Time.deltaTime;
            if (Input.GetMouseButtonDown(0))
            {
                goNakgong = true;
            }

            if (_playerCollider.IsColliding)
            {
                IsGetHooking = false;
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
            NakGong(); 
        }
        sword.GetComponent<Deager>().StartCoroutine("TurnBack");

        IsGetHooking = false;
    }

    IEnumerator Jump()
    {
        _isJumping = true;
        _rigid.velocity = new Vector2(_rigid.velocity.x,InvManager.Instance.jumpPower);
        yield return new WaitForSeconds(0.1f);
          
        while (!_playerCollider.isOnGround)
        {
            if (Input.GetKeyDown(KeyCode.Space) && !IsThrowing)
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
    IEnumerator GroundedChecker()
    {
        //bool tempforbug = _playerCollider.isOnGround;
        yield return new WaitForSeconds(0.04f);
        while (!_playerCollider.isOnGround)
        {
            if (_isReadyNakgong && InvManager.Instance.speed > 0f)
            {
                InvManager.Instance.speed -= 300f * Time.deltaTime;
            }

            if (_isNakGonging)
            {
                
                if (AttackMode == "Default")
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
            if (AttackMode == "Default")
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
