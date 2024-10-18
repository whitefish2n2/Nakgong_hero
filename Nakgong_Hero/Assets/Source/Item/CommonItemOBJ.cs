using System;
using System.Collections;
using System.Collections.Generic;
using Items;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class CommonItemOBJ : MonoBehaviour
{
    public CommonItem itemInfo;
    [SerializeField] bool interactOnUser;
    private bool dontcheck = true;
    public void Watching()
    {
        dontcheck = false;
        StartCoroutine(WatchChecker());
    }

    public void DisWatching()
    {
        dontcheck = true;
        ItemInteract.Instance.InteractOut();
        StopCoroutine(WatchChecker());
    }

    IEnumerator WatchChecker()
    {
        while (Vector2.Distance(transform.position, PlayerController.Instance.playerPos) < 3f)
        {
            if (!dontcheck)
            {
                if(interactOnUser)
                    ItemInteract.Instance.InteractOnHere(new Vector2(PlayerController.Instance.playerPos.x-0.4f, PlayerController.Instance.playerPos.y + 1f));
                else
                    ItemInteract.Instance.InteractOnHere(new Vector2(transform.position.x, transform.position.y + 1f));
                if (Input.GetKeyDown(KeyCode.F))
                {
                    itemInfo.Get();
                    DisWatching();
                    if(itemInfo.isDestroyItem)
                        Destroy(gameObject);
                }
            }
            yield return null;
        }
        DisWatching();
    }
}
