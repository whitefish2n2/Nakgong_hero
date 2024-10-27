using UnityEngine;
using Random = UnityEngine.Random;

namespace Source.Item
{
    public class InvManager : MonoBehaviour
    {
        public static InvManager Instance;
        //플레이어 속도
        public float speed;
        public float startSpeed;
        public float shiftSpeedPlus;
        //플레이어 점프력
        public float jumpPower;
        //플레이어 공격력
        public float attackPower;
        //플레이어 스탠스 파괴율
        public float stans;
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
            attackPower = 3f;//저장 파일에서 저장된 기본값 받아오자-
            stans = 3f;//몬스터의 스탠스 수치를 얼마나 깎나/기본값
            hp = 50f;
            maxHp = 100f;
            jumpPower = 5f;
            startSpeed = 300f;
            shiftSpeedPlus = 100f;
            speed = startSpeed;
            startGravityScale = 1f;
            gravityScalePlus = 10f;
            airBonePower = 0f;
            stans = 100f;
            difficulty = 1f;
            Instance = this;
        }
        public void GetHone()
        {
            attackPower += 5f;
        }

        public void GetWeight()
        {
            gravityScalePlus += 0.5f;
        }
        public void GetRedPortion()
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
    }
}
