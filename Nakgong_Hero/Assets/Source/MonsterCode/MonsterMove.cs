using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class MonsterMove : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private Animator anim;
    [SerializeField] private Vector2 aggroRange;
    [SerializeField] private float aggroSpeed;
    [SerializeField] private Vector2 attackBoxSize;
    [SerializeField] private float attackSpeed; 
    public float damage;
    public float stunTime;
    private Vector2 localscale;
    [SerializeField]private Rigidbody2D rigid;
    private bool isAggroling;
    private bool watchingLeft = true;
    private int nextMove;
    public bool isAttacking;
    private readonly int _isWalking = Animator.StringToHash("isWalking");

    public void Start()
    {
        localscale = transform.localScale;
        //rigid = GetComponent<Rigidbody2D>();
        isAttacking = false;
        Invoke("Think",5f);
    }

    public void FixedUpdate()
    {
        rigid.velocity = new Vector2(nextMove * speed, rigid.velocity.y);
        if (!isAggroling)
        {
            if (math.abs(PlayerController.PlayerPos.x - transform.position.x) < aggroRange.x)
            {
                if (math.abs(PlayerController.PlayerPos.y - transform.position.y) < aggroRange.y)
                {
                    isAggroling = true;
                    StartCoroutine("Aggro");
                }
            }
            Vector2 front = new Vector2(rigid.position.x + nextMove, rigid.position.y);
            Debug.DrawRay(front, Vector3.down, new Color(1,0,0));
            RaycastHit2D raycast = Physics2D.Raycast(front, Vector3.down,1,LayerMask.GetMask("Default"));
            if (raycast.collider == null)
            {
                nextMove= nextMove*(-1);
                CancelInvoke();
                Invoke("Think",2); 
                speed = 0.2f;
            }
        }
    }

    public IEnumerator Aggro()
    {
        anim.StopPlayback();
        speed += aggroSpeed;
        CancelInvoke(nameof(Think));
        while (math.abs(PlayerController.PlayerPos.x - transform.position.x) < aggroRange.x && math.abs(PlayerController.PlayerPos.y - transform.position.y) < aggroRange.y)
        {
            if (!isAttacking)
            {
                if (math.abs(PlayerController.PlayerPos.x - transform.position.x) > 0.5f)
                {
                    rigid.velocity = new Vector2(nextMove * speed, rigid.velocity.y);
                    anim.SetBool(_isWalking, true);
                    if (PlayerController.PlayerPos.x - transform.position.x > 0)
                    {
                        watchingLeft = false;
                        nextMove = 1;
                        transform.localScale = new Vector3(-1 * localscale.x, transform.localScale.y);
                    }
                    else
                    {
                        watchingLeft = true;
                        nextMove = -1;
                        transform.localScale = new Vector3(localscale.x, transform.localScale.y);
                    }
                }
                else
                {
                    nextMove = 0;
                    anim.SetBool(_isWalking, false);
                    StartCoroutine(Attack());
                }
            }
            yield return null;
        }

        speed -= aggroSpeed;
        isAggroling = false;
        Invoke(nameof(Think),2f);
        yield break;
    }

    public void Think()
    {
        speed = 1f;
        nextMove = Random.Range(-1, 2);
        Invoke("Think",5f);
        if (nextMove == 1)
        {
            watchingLeft = false;
            transform.localScale = new Vector3(-1*localscale.x, transform.localScale.y);
            anim.SetBool(_isWalking,true);
        }
        else if (nextMove == -1)
        {
            watchingLeft = true;
            transform.localScale = new Vector3(localscale.x, transform.localScale.y);
            anim.SetBool(_isWalking,true);
        }
        else
        {
            anim.SetBool(_isWalking,false);
        }
    }

    public IEnumerator Attack()
    {
        isAttacking = true;
        yield return new WaitForSeconds(attackSpeed);
        Debug.Log("공격하다");
        RaycastHit2D ray = Physics2D.BoxCast(transform.position, new Vector2(0.01f, attackBoxSize.y), 0,
            watchingLeft ? Vector2.left : Vector2.right, attackBoxSize.x, LayerMask.GetMask("Player"));
        if (ray)
        {
            if(ray.transform.CompareTag("Player"))
                PlayerController.GotAttack(damage, stunTime);
        }
        isAttacking = false;
        yield break;
    }
}
