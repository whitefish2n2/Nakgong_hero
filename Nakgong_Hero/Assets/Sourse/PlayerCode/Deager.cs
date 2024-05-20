using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Deager : MonoBehaviour
{
    [SerializeField] private float ThrowingSpeed;
    [SerializeField] private Rigidbody2D rigid;
    public static bool isCrashWithWall = false;
    private bool dontCheck = false;
    private bool isThrowing = false;
    public void ThrowAt_withThrowRange(Vector3 ThrowPos, float Range)
    {
        StartCoroutine(ThrowAt(ThrowPos, Range));
    }
    private IEnumerator ThrowAt(Vector3 ThrowPos_IE, float Range_IE)
    {
        isThrowing = true;
        float elapsedTime = 0f;
        Vector3 playerPos = PlayerController.PlayerPos;
        Vector2 dirvec = ThrowPos_IE - playerPos;
        float MouseRange = math.sqrt(math.pow(math.abs(ThrowPos_IE.x - playerPos.x), 2) +
                                   math.pow(math.abs(ThrowPos_IE.y - playerPos.y), 2));
        gameObject.transform.up = dirvec.normalized;
        while (math.sqrt(math.pow(math.abs(transform.position.x - playerPos.x), 2) +
                                             math.pow(math.abs(transform.position.y - playerPos.y), 2)) < Range_IE && math.sqrt(math.pow(math.abs(transform.position.x - playerPos.x), 2) +
                   math.pow(math.abs(transform.position.y - playerPos.y), 2)) < MouseRange)
        {
            if (!isCrashWithWall && PlayerController.isThrowing && !PlayerController.isGetHooking)
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
        rigid.velocity = rigid.velocity - dirvec * ThrowingSpeed;
        yield break;
    }

    private void Update()
    {
        if (!PlayerController.isThrowing)
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
