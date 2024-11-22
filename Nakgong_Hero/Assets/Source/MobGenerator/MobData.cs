using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Source.MobGenerator
{
    public class MobData : MonoBehaviour
    {
        public static MobData instance;
        public Queue<GameObject>[] Mobs;
        public GameObject[] mobPrefabs;
        public int[] mobPoolCount;
        [HideInInspector] public bool[] toggled;
        public GameObject getMob(MobType type)
        {
            return new GameObject();

        }
        
        public enum MobType
        {
            Slime,
            RedSlime,
            Observer
        }
    }
}
