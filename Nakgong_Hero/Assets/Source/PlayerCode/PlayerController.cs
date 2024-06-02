using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rigid;
    [SerializeField] private float startSpeed;
    [SerializeField] private float shiftSpeedPlus;
    [SerializeField] private float startGravityScale;
    [SerializeField] private Animator anim;
    [SerializeField] private GameObject AttackBox;
    [Header("대검 투척")]
    [SerializeField] private GameObject Sword;
    [SerializeField] private float ChainLength;
    [SerializeField] private float GetHookSpeed;
    [SerializeField] int ThrowOnAirCount;

    private int ThrowOnAirCountTemp;
    //땅에 닿았는지를 판별하는 class
    private PlayerCollider _playerCollider;
    //수평이동
    private float horizontal;
    //인게임 내 적용 speed
    public static float speed;
    //인게임 내 적용 점프력
    public static float jumpPower;
    //점프 중/ 낙공 중 판별 bool
    private bool isNakGonging = false;
    private bool isjumping = false;
    public static bool isThrowing = false;
    public static bool isGetHooking = false;
    public static float AttackPower;
    public static float stans;
    public static string AttackMode;
    public static Vector3 PlayerPos;
    public static Quaternion PlayerRotate;
    public static float AirBonePower;
    private bool goingleft;
    //이전 프레임에 보고 있는 ITEM OBJECT
    private Collider2D WatchingItemTemp;
    private void Start()
    {
        ThrowOnAirCountTemp = ThrowOnAirCount;
        jumpPower = 300f;
        PlayerPos = transform.position;
        AttackMode = "Default";
        AttackBox.SetActive(false);
        speed = startSpeed;
        _playerCollider = GetComponent<PlayerCollider>();
        AttackPower = 3f;//저장 파일에서 저장된 기본값 받아오자-
        stans = 3f;//몬스터의 스탠스 수치를 얼마나 깎나/기본값
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && _playerCollider.isOnGround)
        {
            rigid.AddForce(Vector2.up * jumpPower);
            isjumping = true;
            StartCoroutine(GroundedChecker());
        }
        if (Input.GetKey(KeyCode.LeftShift) && !isjumping)
        {
            speed = startSpeed + shiftSpeedPlus;
            anim.SetFloat("MoveSpeed",2f);
        }
        else
        {
            if (!isjumping)
            {
                speed = startSpeed;
                anim.SetFloat("MoveSpeed", 1f);
            }
        }
        if (Input.GetKey(KeyCode.A)&&!Input.GetKey(KeyCode.D))
        {
            horizontal = -1f;
            gameObject.transform.rotation = new Quaternion(0f, 0f,0f,0f);
            goingleft = true;
            if (!isjumping && anim.GetCurrentAnimatorStateInfo(0).IsName("LeftMove") == false)
            {
                anim.Play("LeftMove");
            }
        }
        if (Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A))
        {
            horizontal = 1f;
            gameObject.transform.rotation = new Quaternion(0f, 180f,0f,0f);
            goingleft = false;
            if (!isjumping && anim.GetCurrentAnimatorStateInfo(0).IsName("LeftMove") == false)
            {
                anim.Play("LeftMove");
            }
        }

        if (!Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A))
        {
            horizontal = 0f;
        }
        if(isjumping || !Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.A))
        {
            if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Default"))
            {
                anim.enabled = false;
                anim.enabled = true;
                anim.Play("Default");
            }
        }
        if (Input.GetMouseButtonDown(0))
        {
            if (!_playerCollider.isOnGround)
            {
                NakGong();
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            if (!isNakGonging && !isGetHooking)
            {
                Throwing();
            }
        }
        RaycastHit2D raycast = Physics2D.Raycast(new Vector2(PlayerPos.x, PlayerPos.y-1f), (goingleft ? Vector3.left : Vector3.right),2,LayerMask.GetMask("Items"));
        if(!raycast.collider) return;
        if (raycast.collider.gameObject.CompareTag("CommonItem"))
        {
            if (WatchingItemTemp != raycast.collider)
            {
                WatchingItemTemp.GetComponent<CommonItemOBJ>().DisWatching();
                raycast.collider.GetComponent<CommonItemOBJ>().Watching();
            }
            WatchingItemTemp = raycast.collider;
            if (Input.GetKeyDown(KeyCode.F))
            {
                Destroy(raycast.collider.gameObject);
            }
        }
    }
    private void FixedUpdate()
    {
        PlayerPos = gameObject.transform.position;
        PlayerRotate = gameObject.transform.rotation;
        rigid.velocity = new Vector2(horizontal * speed * Time.deltaTime, rigid.velocity.y);
    }

    private void NakGong()
    {
        if (!isNakGonging && !isGetHooking && !isThrowing && isjumping)
        {
            CameraDefaultMove.CameraposPlus = -2f;
            AttackBox.SetActive(true);
            isNakGonging = true;
            rigid.gravityScale = rigid.gravityScale += 10f;
            StartCoroutine(GroundedChecker());
        }
    }

    private void Throwing()
    {
        if (!isThrowing)
        {
            if (isjumping)
            {
                if (ThrowOnAirCount > 0)
                {
                    ThrowOnAirCount--;
                    isThrowing = true;
                    Vector3 MousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,
                        Input.mousePosition.y, 0f));
                    Sword.GetComponent<Deager>().ThrowAt_withThrowRange(MousePos, ChainLength);
                }
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
        isGetHooking = true;
        Vector3 StartPos = PlayerPos;
        Vector3 GetHere = Sword.transform.position;
        float elapsedTime = 0f;
        float gravityTemp = rigid.gravityScale;
        rigid.gravityScale = 0f;
        while (elapsedTime < GetHookSpeed && !_playerCollider.isOnGround && isGetHooking)
        {
            gameObject.transform.position = Vector3.Lerp(StartPos, GetHere, elapsedTime / GetHookSpeed);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        rigid.gravityScale = gravityTemp;
        if (_playerCollider.isOnGround)
        {
            Sword.GetComponent<Deager>().StartCoroutine("TurnBack");
        }
    }
    IEnumerator GroundedChecker()
    {
        //bool tempforbug = _playerCollider.isOnGround;
        yield return new WaitForSeconds(0.04f);
        Debug.Log(_playerCollider.isOnGround);
        while (!_playerCollider.isOnGround)
        {
            if (isjumping && speed > 0f)
            {
                speed -= 300f * Time.deltaTime;
            }

            if (isNakGonging)
            {
                if (AttackMode == "Default")
                {
                    AirBonePower += 300f * Time.deltaTime;
                }
            }
            yield return null;
        }
        if (isjumping)
        {
            StopCoroutine("jumpSlower");
            isjumping = false;
            speed = startSpeed;
        }
        if (isNakGonging)
        {
            isNakGonging = false;
            if (AttackMode == "Default")
            {
                AirBonePower = 0f;
                AttackBox.SetActive(false);
            }
        }
        CameraDefaultMove.CameraposPlus = 0f;
        ThrowOnAirCount = ThrowOnAirCountTemp;
        rigid.gravityScale = startGravityScale;
        yield break;
    }
}
