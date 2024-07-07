using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollider : MonoBehaviour
{
    private bool isOnGroundThis = false;
    private bool detect;
    private bool isOnCoverThis;

    public bool isOnGround
    {
        get
        {
            if (isOnGroundThis)
            {
                isOnGroundThis = false;
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            isOnGroundThis = true;
        }
        else if (other.gameObject.CompareTag("Cover") || other.gameObject.CompareTag("MovingObject"))
        {
            isOnCoverThis = true;
        }
        else
        {
            isOnCoverThis = false;
        }
    }
}
