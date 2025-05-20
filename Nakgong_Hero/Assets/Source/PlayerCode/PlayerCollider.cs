using System;
using UnityEngine;

namespace Source.PlayerCode
{
    public class PlayerCollider : MonoBehaviour
    {
        private bool _colliding;
        private static Collider2D _collider;
        private static Vector2 _rayBox;

        private void Start()
        {
            Init();
        }

        private void Init()
        {
            _collider = GetComponent<Collider2D>();
            _rayBox = new Vector2(_collider.bounds.min.x-_collider.bounds.min.x, 0.1f);
        }

        public static bool IsOnGround
        {
            get
            {
                Vector2 rayOrigin =
                    new Vector2(PlayerController.instance.playerPos.x, PlayerController.instance.playerPos.y);
                
                var ray = Physics2D.OverlapBox(rayOrigin,_rayBox,
                    0, LayerMask.GetMask("Default","MovingObjects"));
                if (ray)
                {
                    if (ray.CompareTag("Ground"))
                    {
                        PlayerController.instance.movingFloor = null;
                        return true;
                    }
                    else if (ray.CompareTag("MovingFloor"))
                    {
                        PlayerController.instance.movingFloor = ray.transform.gameObject;
                        return true;
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
