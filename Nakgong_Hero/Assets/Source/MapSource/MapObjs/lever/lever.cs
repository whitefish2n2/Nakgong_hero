using Source.Item;
using Source.MapSource.MapObjs.lever;
using UnityEngine;
using UnityEngine.Events;

namespace Source.MapSource.MapObjs
{
    public class Lever : CommonItemOBJ
    {
        [SerializeField] private UnityEvent onDisActive;
        [SerializeField] private bool isUsed;
        private SpriteRenderer spriteRenderer;
        [SerializeField] private LeverData leverData;

        private void Awake()
        {
            spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        }

        public override void Get()
        {
            if (!isUsed)
            {
                base.Get();
                spriteRenderer.sprite = leverData.AfterActive;
                isUsed = true;
            }
            else
            {
                if (onDisActive == null) return;
                onDisActive.Invoke();
                isUsed = false;
                spriteRenderer.sprite = leverData.BeforeActive;
            }
        }
    }
}
