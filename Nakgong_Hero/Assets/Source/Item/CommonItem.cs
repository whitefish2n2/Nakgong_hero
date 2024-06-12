using System;
using System.Collections;
using System.Collections.Generic;
using Items;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using Object = System.Object;

[Serializable]
public class CommonItem
{
    public CommonItemType ItemType;
    public string ItemName;
    public GameObject prefab;
    public Sprite InvSprite;
    [TextArea] public string Discription;
}

