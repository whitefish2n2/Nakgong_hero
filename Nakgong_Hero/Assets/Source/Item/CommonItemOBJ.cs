using System;
using System.Collections;
using System.Collections.Generic;
using Items;
using Source.PlayerCode;
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

    public void Get()
    {
        itemInfo.Get();
        DisWatching();
        if(itemInfo.isDestroyItem)
            Destroy(gameObject);
    }

    IEnumerator WatchChecker()
    {
        while (!dontcheck)
        {
            ItemInteract.Instance.InteractOnHere(interactOnUser
                ? new Vector2(PlayerController.Instance.playerPos.x - 0.4f, PlayerController.Instance.playerPos.y + 1f)
                : new Vector2(transform.position.x, transform.position.y + 1f));
            yield return null;
        }
        DisWatching();
    }
}
