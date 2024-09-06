using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollider : MonoBehaviour
{
    private bool Colliding = false;
    public bool isOnGround
    {
        get
        {
            RaycastHit2D leftRay =
                Physics2D.Raycast(new Vector2(PlayerController.PlayerPos.x-0.5f, PlayerController.PlayerPos.y - 1f),
                    Vector2.down, 0.2f, LayerMask.GetMask("Default"));
            RaycastHit2D rightRay =
                Physics2D.Raycast(new Vector2(PlayerController.PlayerPos.x+0.5f, PlayerController.PlayerPos.y - 1f),
                    Vector2.down, 0.2f, LayerMask.GetMask("Default"));
            if (leftRay.collider is not null)
            {
                if (leftRay.collider.gameObject.CompareTag("Ground"))
                {
                    PlayerController.movingFloor = null;
                    return true;
                }
                if (leftRay.collider.gameObject.CompareTag("MovingFloor"))
                {
                    PlayerController.movingFloor = leftRay.transform.gameObject;
                    return true;
                }
                else
                {
                    PlayerController.movingFloor = null;
                }
            }
            else if (rightRay.collider is not null)
            {
                if (rightRay.collider.gameObject.CompareTag("Ground"))
                {
                    PlayerController.movingFloor = null;
                    return true;
                }
                if (rightRay.collider.gameObject.CompareTag("MovingFloor"))
                {
                    PlayerController.movingFloor = rightRay.transform.gameObject;
                    return true;
                }
                PlayerController.movingFloor = null;
            }
            else
            {
                return false;
            }
            return false;
        }
    }

    public bool IsColliding
    {
        get
        {
            if (Colliding)
            {
                Colliding = false;
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
            Colliding = true;
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
            Colliding = false;
    }
}
