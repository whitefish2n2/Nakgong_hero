using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rigid;
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
    //점프 중/ 낙공 중 판별 bool
    private bool isNakGonging = false;
    private bool isjumping = false;
    public static bool isThrowing = false;
    public static bool isGetHooking = false;
    public static string AttackMode;
    public static Vector3 PlayerPos;
    public static Quaternion PlayerRotate;
    private bool goingleft;
    //이전 프레임에 보고 있는 ITEM OBJECT
    private Collider2D WatchingItemTemp;
    private void Start()
    {
        ThrowOnAirCountTemp = ThrowOnAirCount;
        PlayerPos = transform.position;
        AttackMode = "Default";
        AttackBox.SetActive(false);
        _playerCollider = GetComponent<PlayerCollider>();
        
    }

    private void Update()
    {
        //이동/애니메이션 재생
        if (Input.GetKeyDown(KeyCode.Space) && _playerCollider.isOnGround)
        {
            rigid.AddForce(Vector2.up * InvManager.Instance.jumpPower);
            isjumping = true;
            StartCoroutine(GroundedChecker());
        }
        if (Input.GetKey(KeyCode.LeftShift) && !isjumping)
        {
            InvManager.Instance.speed = InvManager.Instance.startSpeed + InvManager.Instance.shiftSpeedPlus;
            anim.SetFloat("MoveSpeed",2f);
        }
        else
        {
            if (!isjumping)
            {
                InvManager.Instance.speed = InvManager.Instance.startSpeed;
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
        //좌클릭-낙공 우클릭-던지기
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
        //아이템 인터렉트 코드
        RaycastHit2D raycast = Physics2D.Raycast(new Vector2(PlayerPos.x, PlayerPos.y-1f), (goingleft ? Vector3.left : Vector3.right),2,LayerMask.GetMask("Items"));
        if (raycast.collider is not null)
        {
            if (raycast.collider.gameObject.CompareTag("CommonItem"))
            {
                if (WatchingItemTemp)
                {
                    WatchingItemTemp.GetComponent<CommonItemOBJ>().DisWatching();
                }
                raycast.collider.GetComponent<CommonItemOBJ>().Watching();
                WatchingItemTemp = raycast.collider;
            }
        }
    }
    private void FixedUpdate()
    {
        PlayerPos = gameObject.transform.position;
        PlayerRotate = gameObject.transform.rotation;
        rigid.velocity = new Vector2(horizontal * InvManager.Instance.speed * Time.deltaTime, rigid.velocity.y);
    }

    private void NakGong()
    {
        if (!isNakGonging && !isGetHooking && !isThrowing && isjumping)
        {
            CameraDefaultMove.CameraposPlus = -2f;
            AttackBox.SetActive(true);
            isNakGonging = true;
            //낙공 속도
            rigid.gravityScale = InvManager.Instance.startGravityScale + InvManager.Instance.GravityScalePlus;
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
                    Sword.GetComponent<SpriteRenderer>().color = Color.white;
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
            if (isjumping && InvManager.Instance.speed > 0f)
            {
                InvManager.Instance.speed -= 300f * Time.deltaTime;
            }

            if (isNakGonging)
            {
                if (AttackMode == "Default")
                {
                    InvManager.Instance.AirBonePower += 300f * Time.deltaTime;
                }
            }
            yield return null;
        }
        if (isjumping)
        {
            StopCoroutine("jumpSlower");
            isjumping = false;
            InvManager.Instance.speed = InvManager.Instance.startSpeed;
        }
        if (isNakGonging)
        {
            isNakGonging = false;
            if (AttackMode == "Default")
            {
                InvManager.Instance.AirBonePower = 0f;
                AttackBox.SetActive(false);
            }
        }
        CameraDefaultMove.CameraposPlus = 0f;
        ThrowOnAirCount = ThrowOnAirCountTemp;
        rigid.gravityScale = InvManager.Instance.startGravityScale;
        yield break;
    }
}
