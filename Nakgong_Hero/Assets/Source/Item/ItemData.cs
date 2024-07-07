using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Items
{
    public class ItemData : MonoBehaviour
    {
        public static ItemData Instance;
        public static GameObject ItemInfoPanel;

        public CommonItem[] CommonItems;
        [HideInInspector] public bool[] toggled;

        private void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            ItemInfoPanel = GameObject.FindWithTag("ItemInfoPanel");
        }

        public CommonItem GetCommonItem(ItemType type) => CommonItems[(int)type];
    }
}
