using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.Properties;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D _rigid;
    private Animator _anim;
    [SerializeField] private GameObject attackBox;
    [FormerlySerializedAs("Sword")]
    [Header("대검 투척")]
    [SerializeField] private GameObject sword;
    [SerializeField] private float chainLength;
    [SerializeField] private float getHookSpeed;
    [SerializeField] int throwOnAirCount;

    private int _throwOnAirCountTemp;
    //땅에 닿았는지를 판별하는 class
    private PlayerCollider _playerCollider;
    //수평이동
    public static float _horizontal;
    //점프 중/ 낙공 중 판별 bool
    private bool IsNakGonging = false;
    private bool IsJumping;
    private bool IsReadyNakgong = false;
    public static bool IsThrowing = false;
    public static bool IsGetHooking = false;
    public static string AttackMode;
    public static Vector3 PlayerPos;
    public static Quaternion PlayerRotate;
    private bool _goingLeft;
    private static bool _isStop;
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
            if (Input.GetKey(KeyCode.LeftShift) && !IsReadyNakgong)
            {
                InvManager.Instance.speed = InvManager.Instance.startSpeed + InvManager.Instance.shiftSpeedPlus;
                _anim.SetFloat(MoveSpeed,2f);
            }
            else
            {
                if (!IsReadyNakgong)
                {
                    InvManager.Instance.speed = InvManager.Instance.startSpeed;
                    _anim.SetFloat(MoveSpeed, 1f);
                }
            }
            if (Input.GetKey(KeyCode.A)&&!Input.GetKey(KeyCode.D))
            {
                GoRight();
            }
            if (Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A))
            {
                GoLeft();
            }
            if (!Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A))
            {
                _horizontal = 0f;
            }
            if(IsReadyNakgong || !Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.A))
            {
                if (!_anim.GetCurrentAnimatorStateInfo(0).IsName("Default"))
                {
                    _anim.StopPlayback();
                    _anim.Play("Default");
                }
            }
            //좌클릭-낙공 우클릭-던지기
            if (Input.GetMouseButtonDown(0))
            {
                if (IsReadyNakgong)
                {
                    NakGong();
                }
            }
            if (Input.GetMouseButtonDown(1))
            {
                if (!IsNakGonging && !IsGetHooking && !IsReadyNakgong)
                {
                    Throwing();
                }
            }
        }
        //아이템 인터렉트 코드
        RaycastHit2D raycast = Physics2D.Raycast(new Vector2(PlayerPos.x, PlayerPos.y-1f), (_goingLeft ? Vector3.left : Vector3.right),2,LayerMask.GetMask("Items","MovingObjects"));
        if (raycast.collider is not null)
        {
            if (raycast.collider.gameObject.CompareTag("CommonItem"))
            {
                ItemInteract.Instance.ChangeText("획득");
                if (_watchingItemTemp)
                {
                    _watchingItemTemp.GetComponent<CommonItemOBJ>().DisWatching();
                }
                raycast.collider.GetComponent<CommonItemOBJ>().Watching();
                _watchingItemTemp = raycast.collider;
            }
            else if (raycast.collider.gameObject.CompareTag("MovingObject"))
            {
                ItemInteract.Instance.ChangeText("상호작용");
                if (_watchingItemTemp)
                {
                    _watchingItemTemp.GetComponent<CommonItemOBJ>().DisWatching();
                }
                raycast.collider.GetComponent<CommonItemOBJ>().Watching();
                _watchingItemTemp = raycast.collider;
            }
        }
    }
    private void FixedUpdate()
    {
        PlayerPos = gameObject.transform.position;
        PlayerRotate = gameObject.transform.rotation;
        _rigid.velocity = new Vector2(_horizontal * InvManager.Instance.speed * Time.deltaTime, _rigid.velocity.y);

            
    }
    public static void Stop()
    {
        _isStop = true;
        _horizontal = 0f;
    }

    public static void DisStop()
    {
        _isStop = false;
    }
    
    private void GoLeft()
    {
        _horizontal = 1f;
        gameObject.transform.rotation = new Quaternion(0f, 180f,0f,0f);
        _goingLeft = false;
        if (!IsReadyNakgong && _anim.GetCurrentAnimatorStateInfo(0).IsName("LeftMove") == false)
        {
            _anim.Play("LeftMove");
        }
    }

    private void GoRight()
    {
        _horizontal = -1f;
        gameObject.transform.rotation = new Quaternion(0f, 0f,0f,0f);
        _goingLeft = true;
        if (!IsReadyNakgong && _anim.GetCurrentAnimatorStateInfo(0).IsName("LeftMove") == false)
        {
            _anim.Play("LeftMove");
        }
    }
    private void NakGong()
    {
        if (!IsNakGonging && !IsGetHooking && !IsThrowing && IsReadyNakgong)
        {
            CameraDefaultMove.CameraposPlus = -2f;
            attackBox.SetActive(true);
            IsNakGonging = true;
            //낙공 속도
            _rigid.gravityScale = InvManager.Instance.startGravityScale + InvManager.Instance.GravityScalePlus;
            StartCoroutine(GroundedChecker());
        }
    }

    private void Throwing()
    {
        if (!IsThrowing)
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
        else
        {
            Deager.isCrashWithWall = false; 
            StartCoroutine(GetHook());
        }
    }
    //대검쪽으로 이동 코루틴
    IEnumerator GetHook()
    {
        IsGetHooking = true;
        Vector3 startPos = PlayerPos;
        Vector3 getHere = sword.transform.position;
        float elapsedTime = 0f;
        float gravityTemp = _rigid.gravityScale;
        bool goNakgong = false;
        _rigid.gravityScale = 0f;
        while (elapsedTime < getHookSpeed && !_playerCollider.isOnGround && IsGetHooking)
        {
            gameObject.transform.position = Vector3.Lerp(startPos, getHere, elapsedTime / getHookSpeed);
            elapsedTime += Time.deltaTime;
            if (Input.GetMouseButtonDown(0))
            {
                goNakgong = true;
            }
            yield return null;
        }
        _rigid.gravityScale = gravityTemp;
        if (goNakgong)
        {
            IsReadyNakgong = true;
            _rigid.velocity = new Vector2(_rigid.velocity.x,InvManager.Instance.jumpPower/2);
            NakGong();    
        }
        if (_playerCollider.isOnGround)
        {
            sword.GetComponent<Deager>().StartCoroutine("TurnBack");
        }
    }

    IEnumerator Jump()
    {
        IsJumping = true;
        _rigid.velocity = new Vector2(_rigid.velocity.x,InvManager.Instance.jumpPower);
        yield return new WaitForSeconds(0.04f);
        Debug.Log(_playerCollider.isOnGround);  
        while (!_playerCollider.isOnGround)
        {
            if (Input.GetKeyDown(KeyCode.Space) && !IsThrowing)
            {
                IsReadyNakgong = true;
                IsJumping = false;
                _rigid.velocity = new Vector2(_rigid.velocity.x,InvManager.Instance.jumpPower);
                StartCoroutine(GroundedChecker());
                throwOnAirCount = _throwOnAirCountTemp;
                yield break;
            }
            yield return null;
        }
        throwOnAirCount = _throwOnAirCountTemp;
    }
    IEnumerator GroundedChecker()
    {
        //bool tempforbug = _playerCollider.isOnGround;
        yield return new WaitForSeconds(0.04f);
        Debug.Log(_playerCollider.isOnGround);
        while (!_playerCollider.isOnGround)
        {
            if (IsReadyNakgong && InvManager.Instance.speed > 0f)
            {
                InvManager.Instance.speed -= 300f * Time.deltaTime;
            }

            if (IsNakGonging)
            {
                if (AttackMode == "Default")
                {
                    InvManager.Instance.AirBonePower += 300f * Time.deltaTime;
                }
            }
            yield return null;
        }
        if (IsReadyNakgong)
        {
            IsReadyNakgong = false;
            InvManager.Instance.speed = InvManager.Instance.startSpeed;
        }
        if (IsNakGonging)
        {
            IsNakGonging = false;
            if (AttackMode == "Default")
            {
                InvManager.Instance.AirBonePower = 0f;
                attackBox.SetActive(false);
            }
        }
        CameraDefaultMove.CameraposPlus = 0f;
        _rigid.gravityScale = InvManager.Instance.startGravityScale;
    }
}
