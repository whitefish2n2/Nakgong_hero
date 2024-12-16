using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace Source.Item
{
    public class WoodBox : MonoBehaviour
    {
        [SerializeField] private UnityEvent onOpen;
        [SerializeField] private int repeatCount = 1;
        [SerializeField] private bool isOpened; 
        [SerializeField] private Sprite openSprite;
        [SerializeField] private bool isInfinityBox;
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (isOpened) return;
            if (!other.gameObject.CompareTag("AttackBox")) return;
            if(!isInfinityBox)
                isOpened = true;
            for (var i = 0; i < repeatCount; i++)
            {
                onOpen?.Invoke();
            }
            gameObject.GetComponent<SpriteRenderer>().sprite = openSprite;
        }
    }
}
