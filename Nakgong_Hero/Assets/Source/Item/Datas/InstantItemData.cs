using UnityEngine;

namespace Source.Item
{
    public class InstantItemData : MonoBehaviour
    {
        public static InstantItemData instance;

        public CommonItem[] CommonItems;
        [HideInInspector] public bool[] toggled;

        private void Awake()
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            foreach (var o in CommonItems)
            {
                o.prefab.GetComponent<CommonItemOBJ>().itemInfo = o;
            }
        }

        public GameObject GetCommonItem(InstantItemType type) => CommonItems[(int)type].prefab;
    }
}
