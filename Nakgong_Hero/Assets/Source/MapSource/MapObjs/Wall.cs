using Source.MapSource.MapMoveManager;
using UnityEngine;

namespace Source.MapSource.MapObjs
{
    public class Wall : MonoBehaviour
    {
        [Header("상대적 움직임.")] [SerializeField] private Vector2 moveTo;
        [SerializeField] private float moveTime;
        [SerializeField] private bool stopPlayer;
        private Vector2 startPosition;
        private bool moved = false;
        [SerializeField] private bool destroyOnEnd;
        private void Awake()
        {
            startPosition = transform.position;
            moveTo = new Vector2(startPosition.x + moveTo.x, startPosition.y + moveTo.y);
        }

        public void WallMove()
        {
            if (moved)
            {
                gameObject.GetComponent<MovingObject>().MoveStart(startPosition, moveTime,stopPlayer);
                moved = false;
            }
            else
            {
                gameObject.GetComponent<MovingObject>().MoveStart(moveTo, moveTime, stopPlayer, destroyOnEnd);
                moved = true;
            }
        
        }
    }
}
