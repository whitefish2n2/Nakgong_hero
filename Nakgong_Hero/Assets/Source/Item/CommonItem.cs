using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using Object = System.Object;

[Serializable]
public class CommonItem
{
    public string ItemName;
    public GameObject prefab;
    public Sprite InvSprite;
    [TextArea] public string Discription;
}

