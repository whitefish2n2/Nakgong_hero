using Source.UiCode;
using UnityEngine;

namespace Source.Item
{
    public class ItemInInventory : MonoBehaviour
    {
        public CommonItem itemInfo; 
        
        public void OnHover()
        {
            InventoryUiManager.Instance.HoverItem(this);
        }

        public void OffHover()
        {
            InventoryUiManager.Instance.DisHoverItem();
        }

        public void Init(CommonItem item = null)
        {
            itemInfo = item;
        }

        public void Remove()
        {
            itemInfo.Remove();
        }
    }
}
