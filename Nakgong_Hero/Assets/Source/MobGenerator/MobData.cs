using System;
using System.Collections.Generic;
using Source.MonsterCode;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

namespace Source.MobGenerator
{
    public class MobData : MonoBehaviour
    {
        public static MobData instance;
        public Queue<GameObject>[] mobs;
        public GameObject[] mobPrefabs;
        public int[] mobPoolCount;
        [HideInInspector] public bool[] toggled;

        private void Awake()
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public GameObject GetMob(MobType type, Vector2 pos = default)
        {
            if (mobs[(int)type].Count < 1)
            {
                var newMob = Instantiate(mobPrefabs[(int)type], pos, quaternion.identity);
                return newMob;
            }
            else
            {
                var newMob = mobs[(int)type].Dequeue();
                newMob.GetComponent<DefaultMonster>().Init();
                newMob.SetActive(true);
                return newMob;
            }
        }

        public void ReturnMob(MobType type, GameObject obj)
        {
            if (mobs[(int)type].Count <= mobPoolCount[(int) type])
            {
                obj.SetActive(false);
                mobs[(int)type].Enqueue(obj);
            }
            else
            {
                Destroy(obj);
            }
        }
        
        public enum MobType
        {
            Slime,
            RedSlime,
            Observer
        }
    }
}
