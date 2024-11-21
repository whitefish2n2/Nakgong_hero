using UnityEngine;

namespace Source.MonsterCode
{
    [CreateAssetMenu(fileName = "MonsterData", menuName = "Scriptable Object/MonsterData", order = int.MaxValue)]
    public class MonsterData : ScriptableObject
    {
        [SerializeField]
        private float maxHp;
        public float MaxHp => maxHp;

        [SerializeField]
        private float damage;
        public float Damage => damage;

        [SerializeField]
        private float defend;
        public float Defend => defend;

        [SerializeField]
        private float speed;
        public float Speed => speed;
        
        [SerializeField]
        private float stunPower;
        public float StunPower => stunPower;
    }
}
