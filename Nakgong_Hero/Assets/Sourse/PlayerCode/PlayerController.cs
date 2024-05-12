using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rigid;
    [SerializeField] private float startSpeed;
    [SerializeField] private float shiftSpeedPlus;
    [SerializeField] private float startGravityScale;
    [SerializeField] private Animator anim;
    //땅에 닿았는지를 판별하는 class
    private PlayerCollider _playerCollider;
    //수평이동
    private float horizontal;
    //인게임 내 적용 speed
    public static float speed;
    //인게임 내 적용 점프력
    public float jumpPower;
    //점프 중/ 낙공 중 판별 bool
    private bool isNakGonging = false;
    private bool isjumping = false;
    private void Start()
    {
        speed = startSpeed;
        _playerCollider = GetComponent<PlayerCollider>();
        Debug.Log(rigid.gravityScale);
    }

    private void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        if (Input.GetKeyDown(KeyCode.Space) && _playerCollider.isOnGround)
        {
            rigid.AddForce(Vector2.up * jumpPower);
            isjumping = true;
            StartCoroutine(GroundedChecker());
        }

        if (Input.GetKey(KeyCode.A))
        {
            gameObject.transform.rotation = new Quaternion(0f, 0f,0f,0f);
            if (Input.GetKey(KeyCode.LeftShift) && !isjumping)
            {
                speed = startSpeed + shiftSpeedPlus;
            }
            if (!isjumping && anim.GetCurrentAnimatorStateInfo(0).IsName("LeftMove") == false)
            {
                anim.Play("LeftMove");
            }
        }
        if (Input.GetKey(KeyCode.D))
        {
            gameObject.transform.rotation = new Quaternion(0f, 180f,0f,0f);
            if (Input.GetKey(KeyCode.LeftShift) && !isjumping)
            {
                speed = startSpeed + shiftSpeedPlus;
            }
            if (!isjumping && anim.GetCurrentAnimatorStateInfo(0).IsName("LeftMove") == false)
            {
                anim.Play("LeftMove");
            }
        }
        if(isjumping || !Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A))
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
    }
    private void FixedUpdate()
    {
        rigid.velocity = new Vector2(horizontal * speed * Time.deltaTime, rigid.velocity.y);
    }

    private void NakGong()
    {
        if (!isNakGonging && isjumping)
        {
            isNakGonging = true;
            rigid.gravityScale = rigid.gravityScale += 2f;
            StartCoroutine(GroundedChecker());
        }
    }
    IEnumerator GroundedChecker()
    {
        //bool tempforbug = _playerCollider.isOnGround;
        yield return new WaitForSeconds(0.04f);
        Debug.Log(_playerCollider.isOnGround);
        while (!_playerCollider.isOnGround)
        {
            if (speed > 0f)
            {
                speed -= 300f * Time.deltaTime;
            }
            yield return null;
        }
        StopCoroutine("jumpSlower");
        isNakGonging = false;
        isjumping = false;
        speed = startSpeed;
        rigid.gravityScale = startGravityScale;
        yield break;
    }
}
