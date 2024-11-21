using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Source.Item
{
    [Serializable]
    public class CommonItem
    {
        public ItemCategory itemType;
        public bool isDestroyItem = true;
        public string ItemName;
        public GameObject prefab;
        public Sprite InvSprite;
        public Sprite invIcon;
        [TextArea] public string Discription;
        public string interactionDescription = "획득";
        public UnityEvent <CommonItem> OnGet;
        public UnityEvent onRemove;

        public void Get()
        {
             OnGet.Invoke(this);
        }

        public void Remove()
        {
            onRemove.Invoke();
        }
    }
}

