using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagePrefabManager : MonoBehaviour
{
    [SerializeField] private GameObject DamagePrefab_Serial;
    public static GameObject DamagePrefab;

    private void Awake()
    {
        DamagePrefab = DamagePrefab_Serial;
    }
}
