using System;
using Source.Item.Datas;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Source.Item
{
    public class CommonItemBox : MonoBehaviour
    {
        public ItemCategory category;
        [SerializeField] private bool isRandom;
        [SerializeField] private bool throwItem;
        [Header("생성 위치(생성자 기준)")]
        [SerializeField] private Vector2 generatePosition;
        [SerializeField] private Quaternion generateRotation;
        [Header("특정 아이템만 생성하기(isRandom = false여야 함, 위 선택한 카테고리에 맞는 아이템이 선택됩니다.)")]
        [SerializeField] private PassiveItemType passiveItem;
        [SerializeField] private ActiveItemType activeItem;
        [SerializeField] private InstantItemType instantItem;
        

        public void Open()
        {
            if (throwItem)
            {
                var instance = Instantiate(MakeItem(), transform.position + (Vector3)generatePosition,
                    new Quaternion(Quaternion.identity.x, Quaternion.identity.y, Random.Range(-0.4f,0.4f), Quaternion.identity.w));
                instance.GetComponent<Rigidbody2D>().AddRelativeForce(Vector3.up * 500f);
            }
            else
            {
                Instantiate(MakeItem(), transform.position + (Vector3)generatePosition,
                    new Quaternion(Quaternion.identity.x, Quaternion.identity.y, Quaternion.identity.z, Quaternion.identity.w));
            }
        }

        private GameObject MakeItem()
        {
            switch (category)
            {
                case ItemCategory.None:
                    break;
                case ItemCategory.Active:
                    return ActiveItemData.instance.GetCommonItem(
                        (ActiveItemType)Random.Range(0, Enum.GetValues(typeof(ActiveItemType)).Length));
                case ItemCategory.Passive:
                    return PassiveItemData.instance.GetCommonItem(
                        (PassiveItemType)Random.Range(0, Enum.GetValues(typeof(PassiveItemType)).Length));
                case ItemCategory.Instance:
                    return InstantItemData.instance.GetCommonItem(
                        (InstantItemType)Random.Range(0, Enum.GetValues(typeof(InstantItemType)).Length));
                case ItemCategory.ActiveAndPassive:
                    return Random.Range(0, Enum.GetValues(typeof(ActiveItemType)).Length + Enum.GetValues(typeof(PassiveItemType)).Length)
                           > Enum.GetValues(typeof(ActiveItemType)).Length
                        ? PassiveItemData.instance.GetCommonItem(
                            (PassiveItemType)Random.Range(0, Enum.GetValues(typeof(PassiveItemType)).Length))
                        : ActiveItemData.instance.GetCommonItem(
                            (ActiveItemType)Random.Range(0, Enum.GetValues(typeof(ActiveItemType)).Length));
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return null;
        }
    }
}
