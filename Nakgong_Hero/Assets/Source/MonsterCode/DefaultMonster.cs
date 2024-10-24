using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Source.MonsterCode
{
    public class DefaultMonster : MonoBehaviour
    {
        [SerializeField]private float HP;
        private float _hpTemp;
        [SerializeField] private float stans; 
        [SerializeField] private float KnockbackForce;
        private Rigidbody2D _thisRigidbody2D;
        private float _stansTemp;
        public float damage;
        [Header("HP_BAR")]
        [SerializeField] private GameObject leftHP_Bar;
        private GameObject _canvas;
        [SerializeField] private float distance;
        private GameObject _leftBarInstance;
        private RectTransform _hpBar;
        private SpriteRenderer _objectSprite;
        private bool _youCantHurtMe;
        private Camera _mainCam;


        private void Start()
        {
            _canvas = GameObject.FindGameObjectWithTag("Canvas");
            _leftBarInstance = Instantiate(leftHP_Bar, _canvas.transform);
            _hpBar = _leftBarInstance.GetComponent<RectTransform>();
            _thisRigidbody2D = gameObject.GetComponent<Rigidbody2D>();
            _objectSprite = gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>();
            _leftBarInstance.gameObject.GetComponent<Image>().color = new Color32(40, 140, 0,255);
            HP *= InvManager.Instance.Difficulty;
            stans *= InvManager.Instance.Difficulty;
            _stansTemp = stans;
            _hpTemp = HP;
            _mainCam = Camera.main;
        }
        private void Update()
        {
            _hpBar.position =
                _mainCam.WorldToScreenPoint(new Vector2(transform.position.x, transform.position.y + distance));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="AttackMode"></param>
        /// <param name="Damage"></param>
        /// <param name="stansMinus"></param>
        public void GotAttack(string AttackMode, float Damage, float stansMinus)
        {
            if (_youCantHurtMe)
            {
                return;
            }
        
            switch (AttackMode)
            {
                case "Default":
                    float realDamage = Damage + InvManager.Instance.AirBonePower/100f;
                    attack_Effect(realDamage);
                    KnockBack(stansMinus);
                    StartCoroutine(Invincibility(0.1f));
                    return;
                case "Throw":
                    attack_Effect(Damage);
                    KnockBack(stansMinus);
                    StartCoroutine(Invincibility(0.3f));
                    break;
            }
        }
        public void GotAttack(string AttackMode, float Damage, float stansMinus, float AirBoneValue)
        {
            if (_youCantHurtMe)
            {
                return;
            }
        
            switch (AttackMode)
            {
                case "대지분쇄":
                    attack_Effect(Damage);
                    AirBone(stansMinus, AirBoneValue);
                    StartCoroutine(Invincibility(0.1f));
                    break;
            }
        }

        private void attack_Effect(float dmg)
        {
            GameObject instance = Instantiate(DamagePrefabManager.DamagePrefab,
                _canvas.transform);
            instance.transform.position =
                _mainCam.WorldToScreenPoint((Vector2)gameObject.transform.position + Random.insideUnitCircle * 0.01f);
            instance.GetComponent<TextMeshProUGUI>().text = ((int)dmg).ToString();
            instance.GetComponent<Animator>().Play("DamageOn");
            Destroy(instance,1f);
            HP -= dmg;
            HpUpdate();
        }

        private void KnockBack(float stanceMinus)
        {
            stans -= stanceMinus;
            if (stans > 0f) return;
            _thisRigidbody2D.AddForce(gameObject.transform.position.x - PlayerController.Instance.playerPos.x > 0
                ? new Vector2(1f * KnockbackForce - stans, InvManager.Instance.AirBonePower)
                : new Vector2(-1f * KnockbackForce, InvManager.Instance.AirBonePower));
            stans = _stansTemp;
        }

        private void AirBone(float stansMinus, float AirBoneValue)
        {
            stans -= stansMinus;
            if (stans > 0f) return;
            _thisRigidbody2D.AddForce(new Vector2(gameObject.transform.position.x - PlayerController.Instance.playerPos.x > 0 ? 0.3f:-0.3f, AirBoneValue));
            stans = _stansTemp;
        }
        private void HpUpdate()
        {
            _leftBarInstance.GetComponent<Image>().fillAmount = HP / _hpTemp;
            if (HP / _hpTemp < 0.3f)
            {
                _leftBarInstance.gameObject.GetComponent<Image>().color = new Color32(180, 0, 0,255);
            }
            else if (HP / _hpTemp < 0.7f)
            {
                _leftBarInstance.gameObject.GetComponent<Image>().color = new Color32(225, 80, 0,255);
            }
            else
            {
                _leftBarInstance.gameObject.GetComponent<Image>().color = new Color32(20, 140, 0,255);
            }
            if (HP <= 0f)
            {
                Dead();
            }
        }
        private void Dead()
        {
            Debug.Log("으앙죽음ㅜㅜ");
            Destroy(gameObject);//죽는 메서드(풀링 관리, 사망 애니메이션 재생)
        }

        private IEnumerator Invincibility(float time)
        {
            _youCantHurtMe = true;
            yield return new WaitForSeconds(time);
            _youCantHurtMe = false;
        }
    }
}
