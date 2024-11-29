using System;
using System.Collections;
using System.Collections.Generic;
using Source.PlayerCode;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Source.MonsterCode.TheEye
{
    public class TheEyeCode : DefaultMonster
    {
        [Header("보스 설정")]
        public GameObject donggong;
        [SerializeField] private float eyeMoveRadius;
        private MobGenerator.MobGenerator eyeMobGenerator; 
        private bool dontMoveEye;
        private bool eyeToMain;
        [SerializeField] private float moveEyeToMainSpeed;
        public GameObject hpBarPrefab;
        private GameObject hpBar;
        private TextMeshProUGUI hpText;
        private Image hpBarImage;
        private Image stansBarImage;
        public float time;
        public static TheEyeCode instance;
        public Animator lightAnimator;
        private static readonly int Shooting = Animator.StringToHash("shooting");
        [SerializeField] private int patternLength;
        [SerializeField] private GameObject smallRazer;
        [SerializeField] private GameObject bigRazer;
        [SerializeField] private PlayableDirector endTimeLine;

        public override void Awake()
        {
            base.Awake();
            instance = this;
            eyeMobGenerator = donggong.GetComponent<MobGenerator.MobGenerator>();
            hpBar = Instantiate(hpBarPrefab,GameStatic.instance.hpCanvas.GetComponent<RectTransform>());
            hpBarImage = hpBar.GetComponentsInChildren<Image>()[2];
            hpText = hpBar.GetComponentInChildren<TextMeshProUGUI>();
            hpText.text = "붉은 눈";
            hpBar.SetActive(false);
        }

        public override void Start()
        {
            base.Start();
        }

        public void StartAction()
        {
            Debug.Log("시작해요");
            hpBar.SetActive(true);
            StartCoroutine(PatternSequence());
        }

        private void Update()
        {
            if (eyeToMain)
            {
                donggong.transform.Translate(new Vector3(0,0,0) * (Time.deltaTime * moveEyeToMainSpeed));
            }
            if (!dontMoveEye)
            {
                Vector2 direction = (PlayerController.instance.playerPos - transform.position).normalized;
                donggong.transform.localPosition = new Vector2(0,0) + direction * eyeMoveRadius;
            }
            
        }

        public override void GotAttack(string attackMode, float damage, float stansMinus)
        {
            if (!isAlive|| !isActiveAndEnabled) return;
            attack_Logic(damage);
        }

        public override void GotAirbornAttack(string attackMode, float damage, float stansMinus, float airBoneValue)
        {
            if (!isAlive|| !isActiveAndEnabled) return;
            attack_Logic(damage);
        }

        protected override void attack_Logic(float dmg)
        {
            StartCoroutine(Invincibility(0.1f));
            GameObject instance = Instantiate(GameStatic.instance.damagePrefab,
                GameStatic.instance.hpCanvas.transform);
            instance.transform.position =
                GameStatic.instance.mainCam.WorldToScreenPoint((Vector2)donggong.transform.position + Random.insideUnitCircle * 0.01f);
            instance.GetComponent<TextMeshProUGUI>().text = ((int)dmg).ToString();
            instance.GetComponent<Animator>().Play("DamageOn");
            Destroy(instance,1f);
            currentHp -= dmg;
            HpUpdate();
        }

        protected override void HpUpdate()
        {
            base.HpUpdate();
            hpBarImage.fillAmount = currentHp / monsterData.MaxHp;
        }

        protected override IEnumerator Attack()
        {
            throw new System.NotImplementedException();
        }

        public override void Dead()
        {
            if(!isAlive) return;
            base.Dead();
            isAlive = false;
            StartCoroutine(Death());
        }

        private IEnumerator Death()
        {
            StopCoroutine(currentPattern);
            endTimeLine.Play();
            yield return new WaitForSeconds(0.5f);
            hpBar.SetActive(false);
        }

        private Coroutine currentPattern;
        private Coroutine currentMiniPattern;
        private IEnumerator PatternSequence()
        {
            while (isAlive)
            {
                var nextPattern = Random.Range(0,3);
                switch (nextPattern)
                {
                    case 0:
                        currentPattern = StartCoroutine(SmallRazerPattern());
                        yield return currentPattern;
                        break;
                    case 1:
                        currentPattern = StartCoroutine(BigRazerPattern());
                        yield return currentPattern;;
                        break;
                    case 2:
                        currentPattern = StartCoroutine(SummonMonsterPattern());
                        yield return currentPattern;
                        break;
                }
            }
        }

        private IEnumerator SmallRazerPattern()
        {
            for (var i = 0; i < 3; i++)
            {
                currentMiniPattern = StartCoroutine(SmallRazerShot());
                yield return currentMiniPattern;
            }
            yield return new WaitForSeconds(1f);
            yield return new WaitForSeconds(1f);
            for (int i = 0; i < 5; i++)
            {
                currentMiniPattern = StartCoroutine(SmallRazerShot());
                yield return new WaitForSeconds(1.5f);
            }
            yield break;
        }

        private IEnumerator SmallRazerShot()
        {
            var razer = Instantiate(smallRazer, donggong.transform.position, Quaternion.identity);
            razer.SetActive(true);
            Destroy(razer,7.5f);
            var razerAnim = razer.GetComponent<Animator>();
            float elapsedTime = 0f;
            razerAnim.Play("Prefer");
            while (elapsedTime < 1.5f)
            {
                elapsedTime += Time.deltaTime;
                razer.transform.up = (PlayerController.instance.playerPos - donggong.transform.position).normalized;
                yield return null;
            }
            dontMoveEye = true;
            yield return new WaitForSeconds(1f);
            lightAnimator.Play("ShotRazer");
            razerAnim.Play("Shot");
            yield return new WaitForSeconds(2f);
            dontMoveEye = false;
            yield return new WaitForSeconds(3f);
            yield break;
        }

        private IEnumerator SummonMonsterPattern()
        {
            for (int i = 0; i < 3; i++)
            {
                eyeMobGenerator.Trigger();
                yield return new WaitForSeconds(1);
            }
        }

        private IEnumerator DropPattern()
        {
            yield break;
        }
        private IEnumerator BigRazerPattern()
        {
            currentMiniPattern = StartCoroutine(BigRazerShot(-transform.right, new Vector3(0, 0, -90), 2f));
            yield return currentMiniPattern;
            currentMiniPattern = StartCoroutine(BigRazerShot(transform.up, new Vector3(0, 0, 180), 2f));
            yield return currentMiniPattern;
            currentMiniPattern = StartCoroutine(BigRazerShot(transform.right, new Vector3(0, 0, 90), 2f));
            yield return currentMiniPattern;
            currentMiniPattern = StartCoroutine(BigRazerShot(-transform.up, new Vector3(0, 0, 0), 2f));
            yield return currentMiniPattern;
        }

        private IEnumerator BigRazerShot(Vector3 dir, Vector3 euler, float beamTime)
        {
            eyeToMain = true;
            var razer = Instantiate(bigRazer, donggong.transform.position, Quaternion.Euler(euler));
            razer.SetActive(true);
            var razerAnim = razer.GetComponent<Animator>();
            razerAnim.Play("Prefer");
            yield return new WaitForSeconds(4f);
            razerAnim.Play("Shot");
            var elapsedTime = 0f;
            razerAnim.SetBool(Shooting,true);
            while (elapsedTime < beamTime)
            {
                elapsedTime += Time.deltaTime;
                var dotValue=Mathf.Cos(Mathf.Deg2Rad*(90*.5f));
                var direction = PlayerController.instance.playerPos - transform.position;
                if (Vector2.Dot(direction.normalized, dir) > dotValue)
                {
                    PlayerController.instance.GotAttack(monsterData.Damage*5f, invincibleTime:0.5f);
                }
                yield return null;
            }
            razerAnim.SetBool(Shooting,false);
            eyeToMain = false;
            razerAnim.Play("End");
        }

        private IEnumerator GroggyPattern()
        {
            yield return new WaitForSeconds(8f);
        }
    }
}
