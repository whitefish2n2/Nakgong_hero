using Source.Item;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Source.MonsterCode
{
    public abstract class DefaultCommonMonster : DefaultMonster
    {
        public float stans;
        public float stansTemp;
        public float knockbackForce;
        public Vector2 aggroRange;
        public float aggroSpeed;
        [Header("HP BAR")]
        [SerializeField] public float distance;
        [HideInInspector] public GameObject leftBarInstance;
        [HideInInspector] public RectTransform hpBar;

        public override void Awake()
        {
            base.Awake();
            leftBarInstance = Instantiate(GameStatic.instance.hpBarPrefabMini, GameStatic.instance.hpCanvas.GetComponent<RectTransform>());
            leftBarInstance.gameObject.GetComponent<Image>().color = new Color32(40, 140, 0,255);
            hpBar = leftBarInstance.GetComponent<RectTransform>();
            stans *= InvManager.instance.difficulty;
            stansTemp = stans;
        }

        private void Update()
        {
            hpBar.position =
                GameStatic.instance.mainCam.WorldToScreenPoint(new Vector2(transform.position.x, transform.position.y + distance));
        }

        // Update is called once per frame
        protected override void HpUpdate()
        {
            leftBarInstance.GetComponent<Image>().fillAmount = currentHp / monsterData.MaxHp;
            if (currentHp / monsterData.MaxHp < 0.3f)
            {
                leftBarInstance.gameObject.GetComponent<Image>().color = new Color32(180, 0, 0,255);
            }
            else if (currentHp / monsterData.MaxHp < 0.7f)
            {
                leftBarInstance.gameObject.GetComponent<Image>().color = new Color32(225, 80, 0,255);
            }
            else
            {
                leftBarInstance.gameObject.GetComponent<Image>().color = new Color32(20, 140, 0,255);
            }
            if (currentHp <= 0f)
            {
                Dead();
            }
            
        }

        protected override void attack_Effect(float dmg)
        {
            GameObject instance = Instantiate(GameStatic.instance.damagePrefab,
                GameStatic.instance.hpCanvas.transform);
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
            Destroy(gameObject);//죽는 메서드(풀링 관리, 사망 애니메이션 재생)
        }
    }
}
