using UnityEngine;

namespace Source.MobGenerator
{
    public class MobData : MonoBehaviour
    {
        public static MobData instance;
        
        public enum MobType
        {
            Slime,
            RedSlime,
            Observer
        }
    }
}
