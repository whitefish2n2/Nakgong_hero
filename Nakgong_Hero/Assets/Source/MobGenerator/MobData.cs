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
        // ReSharper disable once UnassignedReadonlyField
        public Queue<GameObject>[] Mobs;
        public GameObject[] mobPrefabs;
        public int[] mobPoolCount;
        [HideInInspector] public bool[] toggled;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject); // 기존 인스턴스를 유지하고 중복 생성 방지
                return;
            }
            instance = this;
            DontDestroyOnLoad(gameObject);
            
            int mobTypeCount = Enum.GetValues(typeof(MobType)).Length;
            Mobs = new Queue<GameObject>[mobTypeCount];
            for (int i = 0; i < mobTypeCount; i++)
            {
                Mobs[i] = new Queue<GameObject>();
            }

        }

        public GameObject GetMob(MobType type, Vector2 pos = default, quaternion rot = default)
        {
            Debug.Log(Mobs[(int)type].Count);
            if (Mobs[(int)type].Count < 1)
            {
                var newMob = Instantiate(mobPrefabs[(int)type], pos, rot);
                return newMob;
            }
            else
            {
                var newMob = Mobs[(int)type].Dequeue();
                newMob.GetComponent<DefaultMonster>().Init();
                newMob.SetActive(true);
                return newMob;
            }
        }

        public void ReturnMob(MobType type, GameObject obj)
        {
            if (Mobs[(int)type].Count <= mobPoolCount[(int) type])
            {
                obj.SetActive(false);
                Mobs[(int)type].Enqueue(obj);
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
