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
                Vector2[] rayOrigins =
                {
                    new Vector2(PlayerController.instance.playerPos.x - 0.5f,
                        PlayerController.instance.playerPos.y - 1f),
                    new Vector2(PlayerController.instance.playerPos.x + 0.5f,
                        PlayerController.instance.playerPos.y - 1f)
                };

                foreach (var origin in rayOrigins)
                {
                    RaycastHit2D ray = Physics2D.Raycast(origin, Vector2.down, 0.2f, LayerMask.GetMask("Default"));

                    if (ray.collider)
                    {
                        if (ray.collider.CompareTag("Ground"))
                        {
                            PlayerController.instance.movingFloor = null;
                            return true;
                        }
                        if (ray.collider.CompareTag("MovingFloor"))
                        {
                            PlayerController.instance.movingFloor = ray.transform.gameObject;
                            return true;
                        }
                    }
                }

                PlayerController.instance.movingFloor = null;
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
