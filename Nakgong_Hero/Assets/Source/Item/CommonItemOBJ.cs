using System.Collections;
using Source.PlayerCode;
using UnityEngine;
using UnityEngine.Serialization;

namespace Source.Item
{
    public class CommonItemOBJ : MonoBehaviour
    {
        public CommonItem itemInfo;
        public bool interactOnUser;
        [HideInInspector]public bool dontCheck = true;
        public void Watching()
        {
            dontCheck = false;
            StartCoroutine(WatchChecker());
        }

        public void DisWatching()
        {
            dontCheck = true;
            ItemInteract.ItemInteract.Instance.InteractOut();
            StopCoroutine(WatchChecker());
        }

        public virtual void Get()
        {
            itemInfo.Get();
            DisWatching();
            if(itemInfo.isDestroyItem)
                Destroy(gameObject);
        }

        IEnumerator WatchChecker()
        {
            while (!dontCheck)
            {
                ItemInteract.ItemInteract.Instance.InteractOnHere(interactOnUser
                    ? new Vector2(PlayerController.instance.playerPos.x - 0.4f, PlayerController.instance.playerPos.y + 1f)
                    : new Vector2(transform.position.x, transform.position.y + 1f));
                yield return null;
            }
            DisWatching();
        }
    }
}
