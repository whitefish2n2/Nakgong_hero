using System.Collections;
using System.Collections.Generic;
using Source.MonsterCode;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Source.MobGenerator
{
    public class Wave : MobGenerator
    {
        [SerializeField] private UnityEvent onEnd;
        [HideInInspector] public List<DefaultMonster> monsterList;
        public bool waveOnce = true;
        private bool isTrigged;
        public override void Trigger()
        {
            if (waveOnce && isTrigged) return;
            isTrigged = true;
            for (int i = 0; i < genList.Length; i++)
            {
                var mob = MobData.instance.GetMob(genList[i],(Vector2)transform.position + genPos[i], Quaternion.identity);
                var mobCode = mob.GetComponent<DefaultMonster>();
                mobCode.wave = this;
                monsterList.Add(mobCode);
            }
        }

        public void Check()
        {
            if (monsterList.Count == 0)
            {
                onEnd.Invoke();
            }
        }
    }
}
