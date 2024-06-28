using System;
using System.Collections;
using System.Collections.Generic;
using Items;
using Unity.Mathematics;
using UnityEngine;

public class CommonItemOBJ : MonoBehaviour
{
    public CommonItem ItemInfo;
    private bool dontcheck = true;
    public void Watching()
    {
        ItemInteract.Instance.ChangeText("획득");
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
        while (Vector2.Distance(transform.position, PlayerController.PlayerPos) < 3f)
        {
            if (!dontcheck)
            {
                ItemInteract.Instance.InteractOnHere(new Vector2(transform.position.x, transform.position.y + 1f));
                if (Input.GetKeyDown(KeyCode.F))
                {
                    DisWatching();
                    InvManager.Instance.GetInGameItem(ItemInfo);
                    Destroy(gameObject);
                }
            }

            yield return null;
        }
        DisWatching();
    }
}
