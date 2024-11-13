using UnityEngine;

namespace Source.Item.Datas
{
    public class ActiveItemData : MonoBehaviour
    {
        public static ActiveItemData instance;

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

        public GameObject GetCommonItem(ActiveItemType type) => CommonItems[(int)type].prefab;
    }
}
