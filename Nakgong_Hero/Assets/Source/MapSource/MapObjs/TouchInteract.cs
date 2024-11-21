using UnityEngine;
using UnityEngine.Events;

namespace Source.MapSource.MapObjs
{
    public class TouchInteract : MonoBehaviour
    {
        [SerializeField] private UnityEvent onTouch;
        [SerializeField] private bool beDestroy;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.gameObject.CompareTag("Player")) return;
            onTouch?.Invoke();
            if(beDestroy)
                Destroy(gameObject);
        }
    }
}
