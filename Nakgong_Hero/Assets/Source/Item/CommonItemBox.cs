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
    private void Start()
    {
    }

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
        CommonItemType SelectItem = (CommonItemType)Random.Range(0,1);
        ItemPrefeb = ItemData.Instance.CommonItems[(int)SelectItem].prefab;
        ItemPrefeb.GetComponent<CommonItemOBJ>().ItemType = SelectItem;
        ItemPrefeb.GetComponent<CommonItemOBJ>().ItemName = ItemData.Instance.CommonItems[(int)SelectItem].ItemName;
        ItemPrefeb.GetComponent<CommonItemOBJ>().InvSprite = ItemData.Instance.CommonItems[(int)SelectItem].InvSprite;
        ItemPrefeb.GetComponent<CommonItemOBJ>().Discription = ItemData.Instance.CommonItems[(int)SelectItem].Discription;
        return ItemPrefeb;
    }
}
