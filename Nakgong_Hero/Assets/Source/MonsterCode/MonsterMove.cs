using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class MonsterMove : MonoBehaviour
{
    [SerializeField] private Vector2 CanMovePos1;
    [SerializeField] private Vector2 CanMovePos2;
    [SerializeField] private float speed;
    [SerializeField] private Animator anim;
    [SerializeField] private Vector2 AggroRange;
    [SerializeField] private float AggroSpeed;
    private Vector2 localscale;
    [SerializeField]private Rigidbody2D rigid;
    private bool isAggroling;
    private int nextMove;
    private void Start()
    {
        localscale = transform.localScale;
        //rigid = GetComponent<Rigidbody2D>();
        Invoke("Think",5f);
    }

    private void FixedUpdate()
    {
        rigid.velocity = new Vector2(nextMove * speed, rigid.velocity.y);
        if (!isAggroling)
        {
            if (math.abs(PlayerController.PlayerPos.x - transform.position.x) < AggroRange.x)
            {
                if (math.abs(PlayerController.PlayerPos.y - transform.position.y) < AggroRange.y)
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

    IEnumerator Aggro()
    {
        anim.StopPlayback();
        speed += AggroSpeed;
        CancelInvoke("Think");
        while (math.abs(PlayerController.PlayerPos.x - transform.position.x) < AggroRange.x && math.abs(PlayerController.PlayerPos.y - transform.position.y) < AggroRange.y)
        {
            if (math.abs(PlayerController.PlayerPos.x - transform.position.x) > 0.5f)
            {
                rigid.velocity = new Vector2(nextMove * speed, rigid.velocity.y);
                if (PlayerController.PlayerPos.x - transform.position.x > 0)
                {
                    nextMove = 1;
                }
                else
                {
                    nextMove = -1;
                }
                if (nextMove == 1)
                {
                    transform.localScale = new Vector3(-1 * localscale.x, transform.localScale.y);
                    anim.SetBool("isWalking", true);
                }
                else if (nextMove == -1)
                {
                    transform.localScale = new Vector3(localscale.x, transform.localScale.y);
                    anim.SetBool("isWalking", true);
                }
            }
            else
            {
                nextMove = 0;
                anim.SetBool("isWalking",false);
            }
            yield return null;
        }

        speed -= AggroSpeed;
        isAggroling = false;
        Invoke("Think",2f);
    }

    void Think()
    {
        speed = 1f;
        nextMove = Random.Range(-1, 2);
        Invoke("Think",5f);
        if (nextMove == 1)
        {
            transform.localScale = new Vector3(-1*localscale.x, transform.localScale.y);
            anim.SetBool("isWalking",true);
        }
        else if (nextMove == -1)
        {
            transform.localScale = new Vector3(localscale.x, transform.localScale.y);
            anim.SetBool("isWalking",true);
        }
        else
        {
            anim.SetBool("isWalking",false);
        }
    }
}
