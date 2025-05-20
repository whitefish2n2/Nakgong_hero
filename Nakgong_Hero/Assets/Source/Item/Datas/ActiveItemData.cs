using UnityEngine;
using UnityEngine.Serialization;

namespace Source.Item.Datas
{
    public class ActiveItemData : MonoBehaviour
    {
        public static ActiveItemData instance;

        public CommonItem[] commonItems;
        [HideInInspector] public bool[] toggled;

        private void Awake()
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            foreach (var o in commonItems)
            {
                if(o.prefab)
                    o.prefab.GetComponent<CommonItemOBJ>().itemInfo = o;
            }
        }

        public GameObject GetCommonItem(ActiveItemType type) => commonItems[(int)type].prefab;
    }
}
