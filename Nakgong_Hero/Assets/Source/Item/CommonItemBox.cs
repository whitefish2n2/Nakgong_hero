using System;
using System.Collections;
using System.Collections.Generic;
using Items;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;
using Random = UnityEngine.Random;

public class DefaultItem : MonoBehaviour
{
    private GameObject thisItem;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("AttackBox"))
        {
            Open();
        }
    }

    private void Open()
    {
        GameObject Instance = Instantiate(MakeItem(), transform.position,
            new Quaternion(Quaternion.identity.x, Quaternion.identity.y, (Random.Range(-0.4f,0.4f)), Quaternion.identity.w));
        Instance.GetComponent<Rigidbody2D>().AddRelativeForce(Vector3.up * 500f);
    }

    private GameObject MakeItem()
    {
        GameObject ItemPrefeb;
        ItemType SelectItem = (ItemType)Random.Range(0,3);
        ItemPrefeb = ItemData.Instance.CommonItems[(int)SelectItem].prefab;
        ItemPrefeb.GetComponent<CommonItemOBJ>().itemInfo = ItemData.Instance.CommonItems[(int)SelectItem];
        return ItemPrefeb;
    }
}
