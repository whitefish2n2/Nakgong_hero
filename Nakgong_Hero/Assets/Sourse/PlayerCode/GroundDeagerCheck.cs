using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundDeagerCheck : MonoBehaviour
{
    [SerializeField] private GameObject Deager;
    public static bool dontCheck = false;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Deager"))
        {
            if (PlayerController.isThrowing && !PlayerController.isGetHooking)
            {
                global::Deager.isCrashWithWall = true;
            }
        }
    }
}
