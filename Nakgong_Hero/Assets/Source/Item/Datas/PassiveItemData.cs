using UnityEngine;

namespace Source.Item.Datas
{
    public class PassiveItemData : MonoBehaviour
    {
        public static PassiveItemData instance;

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
        public GameObject GetCommonItem(PassiveItemType type) => CommonItems[(int)type].prefab;
    }
}
