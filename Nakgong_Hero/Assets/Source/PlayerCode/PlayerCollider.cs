using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollider : MonoBehaviour
{
    private bool IsOnGround_this = false;

    public bool isOnGround
    {
        get
        {
            if (IsOnGround_this)
            {
                IsOnGround_this = false;
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
            IsOnGround_this = true;
        }
    }
}
