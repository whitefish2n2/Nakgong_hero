using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Deager : MonoBehaviour
{
    [SerializeField] private float ThrowingSpeed;
    [SerializeField] private float TurnBackSpeed;
    [SerializeField] private float AirWaitTime;
    [SerializeField] private Rigidbody2D rigid;
    public static bool isCrashWithWall = false;
    private bool _isThrowing;
    public void ThrowAt_withThrowRange(Vector3 ThrowPos, float Range)
    {
        StartCoroutine(ThrowAt(ThrowPos, Range));
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_isThrowing)
        {
            if (other.CompareTag("DefaultMonster"))
            {
                other.GetComponent<DefaultMonster>().GotAttack("Throw", InvManager.Instance.AttackPower*1371, InvManager.Instance.stans*40);
            }
        }
        
    }

    private IEnumerator ThrowAt(Vector3 ThrowPos_IE, float Range_IE)
    {
        float elapsedTime = 0f;
        Vector3 playerPos = PlayerController.PlayerPos;
        Vector2 dirvec = ThrowPos_IE - playerPos;
        float MouseRange = Vector2.Distance(ThrowPos_IE, playerPos);
        _isThrowing = true;
        gameObject.transform.up = dirvec.normalized;
        while (Vector2.Distance(transform.position,playerPos) < Range_IE && Vector2.Distance(transform.position,playerPos) < MouseRange)
        {
            if (!isCrashWithWall && PlayerController.IsThrowing && !PlayerController.IsGetHooking)
            {
                rigid.velocity = dirvec * ThrowingSpeed;
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            else
            {
                break;
            }
        }
        rigid.velocity -= dirvec * ThrowingSpeed;
        StartCoroutine(TurnBack());
        yield break;
    }
    IEnumerator TurnBack()
    {
        GroundDeagerCheck.dontCheck = true;
        isCrashWithWall = false;
        yield return new WaitForSeconds(AirWaitTime);
        float turnbackTemp = TurnBackSpeed;
        Vector2 dirvec = PlayerController.PlayerPos - transform.position;
        while (Vector2.Distance(transform.position, PlayerController.PlayerPos) > 0.9f)
        {
            dirvec = PlayerController.PlayerPos - transform.position;
            transform.position = Vector2.Lerp(transform.position, PlayerController.PlayerPos, TurnBackSpeed * Time.deltaTime);
            transform.up = dirvec;
            TurnBackSpeed += 3f * Time.deltaTime;
            yield return null;
        }
        TurnBackSpeed = turnbackTemp;
        PlayerController.IsGetHooking = false;
        PlayerController.IsThrowing = false;
        GroundDeagerCheck.dontCheck = false;
        gameObject.GetComponent<SpriteRenderer>().color = Color.clear;
        _isThrowing = false;
        yield break;
    }

    private void Update()
    {
        if (!PlayerController.IsThrowing)
        {
            float zRotPlus = (PlayerController.PlayerRotate.y <= 0.5f) ? -45f : 45f;
            gameObject.transform.position =
                new Vector3(PlayerController.PlayerPos.x,
                PlayerController.PlayerPos.y + 0.26f, 0f);
            gameObject.transform.rotation = Quaternion.Euler(0f,0f,zRotPlus);
        }
    }

    /*
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ground") && !dontCheck)
        {
            isCrashWithWall = true;
        }
    }
    */
}
