using UnityEngine;

namespace Source.PlayerCode
{
    public class PlayerCollider : MonoBehaviour
    {
        private bool _colliding;
        public static bool IsOnGround
        {
            get
            {
                RaycastHit2D leftRay =
                    Physics2D.Raycast(new Vector2(PlayerController.instance.playerPos.x-0.5f, PlayerController.instance.playerPos.y - 1f),
                        Vector2.down, 0.2f, LayerMask.GetMask("Default"));
                RaycastHit2D rightRay =
                    Physics2D.Raycast(new Vector2(PlayerController.instance.playerPos.x+0.5f, PlayerController.instance.playerPos.y - 1f),
                        Vector2.down, 0.2f, LayerMask.GetMask("Default"));
                if (leftRay.collider is not null)
                {
                    if (leftRay.collider.gameObject.CompareTag("Ground"))
                    {
                        PlayerController.instance.movingFloor = null;
                        return true;
                    }
                    if (leftRay.collider.gameObject.CompareTag("MovingFloor"))
                    {
                        PlayerController.instance.movingFloor = leftRay.transform.gameObject;
                        return true;
                    }
                    PlayerController.instance.movingFloor = null;
                }
                else if (rightRay.collider is not null)
                {
                    if (rightRay.collider.gameObject.CompareTag("Ground"))
                    {
                        PlayerController.instance.movingFloor = null;
                        return true;
                    }
                    if (rightRay.collider.gameObject.CompareTag("MovingFloor"))
                    {
                        PlayerController.instance.movingFloor = rightRay.transform.gameObject;
                        return true;
                    }
                    PlayerController.instance.movingFloor = null;
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
                if (_colliding)
                {
                    _colliding = false;
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
                _colliding = true;
            }
        }

        private void OnCollisionExit2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Ground"))
                _colliding = false;
        }
    }
}
