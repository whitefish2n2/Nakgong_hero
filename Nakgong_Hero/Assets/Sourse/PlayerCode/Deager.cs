using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Deager : MonoBehaviour
{
    [SerializeField] private float ThrowingSpeed;
    public bool isCrashWithWall = false;
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
        gameObject.transform.up = dirvec.normalized;
        while (math.sqrt(math.pow(math.abs(transform.position.x - playerPos.x), 2) +
                                             math.pow(math.abs(transform.position.y - playerPos.y), 2)) < Range_IE)
        {
            if (!isCrashWithWall)
            {
                gameObject.transform.position = Vector2.Lerp(playerPos, ThrowPos_IE, elapsedTime / ThrowingSpeed);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            else
            {
                yield break;
            }
        }

        yield break;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            gameObject.transform.position = PlayerController.PlayerPos;
            isThrowing = false;
            isCrashWithWall = false;
        }

        if (!isThrowing)
        {
            float zRotPlus = (PlayerController.PlayerRotate.y <= 0.5f) ? -45f : 45f;
            gameObject.transform.position =
                new Vector3(PlayerController.PlayerPos.x,
                PlayerController.PlayerPos.y + 0.26f, 0f);
            gameObject.transform.rotation = Quaternion.Euler(0f,0f,zRotPlus);
            Debug.Log(gameObject.transform.rotation.z);
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
