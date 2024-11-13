using System.Collections.Generic;
using Source.UiCode;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Source.Item
{
    public class InvManager : MonoBehaviour
    {
        public static InvManager instance;
        //플레이어 속도
        public float speed;
        public float startSpeed;
        public float shiftSpeedPlus;
        //플레이어 점프력
        public float jumpPower;
        //플레이어 공격력
        public float attackPower;
        public float attackPowerMultiple;
        //플레이어 방어력
        public float defense;
        public float defenseMultiple;
        //플레이어 행운(상자 확률 등에 가중치 적용)
        public float luck;
        //대검 던지기 관련 (빨리 던지거나 빨릳 돌아오는)스탯
        public float orb;
        //플레이어 스탠스 파괴율
        public float stansBreak;
        //에어본 세기
        public float airBonePower;
        //그래비티 스케일
        public float gravityScale;
        public float startGravityScale;
        public float gravityScalePlus;
        //체력
        public float hp;
        public float maxHp;
        //골드
        public float gold;
        //난이도
        public float difficulty;
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
            attackPower = 3f;//저장 파일에서 저장된 기본값 받아오자-
            attackPowerMultiple = 1;
            stansBreak = 3f;//몬스터의 스탠스 수치를 얼마나 깎나/기본값
            hp = 100f;
            maxHp = 100f;
            jumpPower = 5f;
            startSpeed = 300f;
            shiftSpeedPlus = 100f;
            speed = startSpeed;
            startGravityScale = 1f;
            gravityScalePlus = 10f;
            airBonePower = 0f;
            stansBreak = 100f;
            gold = 0;
            orb = 0;
            luck = 0;
            defense = 1;
            defenseMultiple = 1;
            difficulty = 1f;
            
        }

        public float GetAttackPower()
        {
            return attackPower * attackPowerMultiple + 1;
        }

        public float GetDefense()
        {
            return defense * defenseMultiple;
        }
        //Instant
        public void GetHone(CommonItem item)
        {
            attackPower += 5f;
        }

        public void GetWeight(CommonItem item)
        {
            gravityScalePlus += 0.5f;
        }
        public void GetRedPortion(CommonItem item)
        {
            if (hp < 100f)
            {
                hp += Random.Range(5, 10);
                if (hp < maxHp)
                {
                    hp = maxHp;
                }
            }
            else
            {
                if (hp < maxHp)
                {
                    hp = maxHp;
                }
            }
        }

        //passive
        public void GetDevilsContract(CommonItem item)
        {
            InventoryUiManager.Instance.AddItem(item);
            attackPowerMultiple *= 1.5f;    
            defenseMultiple *= 0.5f;
        }
        
        //active
    }
}
