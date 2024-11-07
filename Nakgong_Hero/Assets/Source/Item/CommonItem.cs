using System;
using Items;
using UnityEngine;
using UnityEngine.Events;

namespace Source.Item
{
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

        public void Init(CommonItem commonItem)
        {
            ItemType = commonItem.ItemType;
            isDestroyItem = commonItem.isDestroyItem;
            ItemName = commonItem.ItemName;
            prefab = commonItem.prefab;
            InvSprite = commonItem.InvSprite;
            Discription = commonItem.Discription;
            interactionDescription = commonItem.interactionDescription;
            interactionDescription = commonItem.interactionDescription;
            OnGet = commonItem.OnGet;
        }
    }
}

