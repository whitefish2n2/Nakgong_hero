using System;
using Source.Item;
using Source.MobGenerator;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Source.MonsterCode
{
    public abstract class DefaultCommonMonster : DefaultMonster
    {
        public float currentStans;
        public float stansTemp;
        public float knockbackForce;
        public Vector2 aggroRange;
        public float aggroSpeed;
        public float attackRange;
        [Header("HP BAR")]
        [SerializeField] public float distance;
        [HideInInspector] public GameObject leftBarInstance;
        [HideInInspector] public Image leftBar;
        [HideInInspector] public RectTransform hpBar;

        public override void Awake()
        {
            base.Awake();
            leftBarInstance = Instantiate(GameStatic.instance.hpBarPrefabMini, GameStatic.instance.hpCanvas.GetComponent<RectTransform>());
            leftBarInstance.gameObject.GetComponent<Image>().color = new Color32(40, 140, 0,255);
            hpBar = leftBarInstance.GetComponent<RectTransform>();
            hpBar.localScale = (5f / GameStatic.instance.mainCam.orthographicSize) * Vector3.one;
            currentStans *= InvManager.instance.difficulty;
            stansTemp = currentStans;
            leftBar = leftBarInstance.GetComponent<Image>();
            HpUpdate();
        }

        public override void Init()
        {
            base.Init();
            hpBar.localScale = (5f / GameStatic.instance.mainCam.orthographicSize) * Vector3.one;
            currentStans *= InvManager.instance.difficulty;
            currentStans = stansTemp;
            HpUpdate();
        }

        private void Update()
        {
            hpBar.position =
                GameStatic.instance.mainCam.WorldToScreenPoint(new Vector2(transform.position.x, transform.position.y + distance));
        }

        private void OnDestroy()
        {
            if(leftBarInstance)
                Destroy(leftBarInstance);
        }

        // Update is called once per frame
        protected override void HpUpdate()
        {
            leftBar.fillAmount = currentHp / monsterData.MaxHp;
            if (currentHp / monsterData.MaxHp < 0.3f)
            {
                leftBar.color = new Color32(180, 0, 0,255);
            }
            else if (currentHp / monsterData.MaxHp < 0.7f)
            {
                leftBar.color = new Color32(225, 80, 0,255);
            }
            else
            {
                leftBar.color = new Color32(20, 140, 0,255);
            }
            if (currentHp <= 0f)
            {
                Dead();
            }
            
        }

        protected override void attack_Logic(float dmg)
        {
            GameObject instance = Instantiate(GameStatic.instance.damagePrefab,
                GameStatic.instance.hpCanvas.transform);
            GameUtil.instance.SlowMotion(0.35f,0.5f);
            instance.transform.position =
                GameStatic.instance.mainCam.WorldToScreenPoint((Vector2)gameObject.transform.position + Random.insideUnitCircle * 0.01f);
            instance.GetComponent<TextMeshProUGUI>().text = ((int)dmg).ToString();
            instance.GetComponent<Animator>().Play("DamageOn");
            Destroy(instance,1f);
            currentHp -= dmg;
            HpUpdate();
        }
        public override void Dead()
        {
            Debug.Log("으앙죽음ㅜㅜ");
            MobData.instance.ReturnMob(monsterData.Type,gameObject);
            base.Dead();
        }
    }
}
