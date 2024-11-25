using System.Collections;
using Source.PlayerCode;
using UnityEngine;
using UnityEngine.Events;

namespace Source.MapSource.MapMoveManager
{
    public class MovingObject : MonoBehaviour
    {
        [SerializeField] private UnityEvent OnEnd;

        public void MoveStart(Vector2 to, float time, bool stopPlayer = false, bool destroyOnFinish = false)
        {
            StartCoroutine(Move(transform.position, to, time, stopPlayer, destroyOnFinish));
        }
        private void MoveEnd(bool destroyOnFinish)
        {
            OnEnd?.Invoke();
            if(destroyOnFinish)Destroy(gameObject);
        }

        private IEnumerator Move(Vector2 from, Vector2 to, float time, bool stopPlayer, bool destroyOnFinish)
        {
            var elapsedTime = 0f;
            var tagTemp = transform.tag;
            gameObject.transform.tag = "Moving";
            if(stopPlayer)
                PlayerController.instance.Stop();
            while (elapsedTime < time)
            {
                gameObject.transform.position = Vector2.Lerp(from, to, elapsedTime / time);
                elapsedTime += Time.unscaledDeltaTime;
                yield return null;
            }
            gameObject.transform.position = to;
            gameObject.transform.tag = tagTemp;
            if (stopPlayer)
                PlayerController.instance.DisStop();
            MoveEnd(destroyOnFinish);
        }
    }
}
