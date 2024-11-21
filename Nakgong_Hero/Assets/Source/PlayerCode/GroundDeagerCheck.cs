using System;
using System.Collections;
using System.Collections.Generic;
using Source.PlayerCode;
using UnityEngine;

public class GroundDeagerCheck : MonoBehaviour
{
    [SerializeField] private GameObject Deager;
    public static bool dontCheck = false;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Deager") && !dontCheck)
        {
            if (PlayerController.instance.isThrowing && !PlayerController.instance.isGetHooking)
            {
                global::Source.PlayerCode.Deager.isCrashWithWall = true;
            }
        }
    }
}
