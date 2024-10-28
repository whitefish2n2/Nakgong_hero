using System;
using System.Collections;
using System.Collections.Generic;
using Items;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Object = System.Object;

[Serializable]
public class CommonItem
{
    public ItemType ItemType;
    public bool isDestroyItem = true;
    public string ItemName;
    public GameObject prefab;
    public Sprite InvSprite;
    [TextArea] public string Discription;
    public string interactionDescription = "획득";
    public UnityEvent OnGet;

    public void Get()
    {
        OnGet.Invoke();
    }
}

