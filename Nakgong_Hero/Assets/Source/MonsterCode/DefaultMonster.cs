using System.Collections;
using JetBrains.Annotations;
using Source.Item;
using Source.MobGenerator;
using UnityEngine;

namespace Source.MonsterCode
{
    public abstract class DefaultMonster : MonoBehaviour
    {
        public MonsterData monsterData;
        public float currentHp;
        [HideInInspector] public Rigidbody2D thisRigidbody2D;
        [HideInInspector] public bool youCantHurtMe;
        
        protected IEnumerator CurrentAttack;
        
        [Header("Monster Move")]
        public Animator anim;
        [HideInInspector] public float currentSpeed;
        [HideInInspector] public Vector2 localScaleTemp;
        [HideInInspector] public bool isAggroling;
        [HideInInspector] public bool isAttacking;
        [HideInInspector] public bool watchingLeft = true;
        [HideInInspector] public bool isAlive = true;
        [HideInInspector] [CanBeNull] public Wave wave;
        public virtual void Awake()
        {
            Init();
            thisRigidbody2D = gameObject.GetComponent<Rigidbody2D>();
            gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>();
        }

        public virtual void Init()
        {
            gameObject.SetActive(true);
            currentHp = monsterData.MaxHp;
            currentHp *= InvManager.instance.difficulty;
            currentSpeed = monsterData.Speed;
            localScaleTemp = transform.localScale;
            isAggroling = false;
            isAttacking = false;
            isAlive = true;
            CurrentAttack = null;
            youCantHurtMe = false;
        }
        public virtual void Start()
        {
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="attackMode"></param>
        /// <param name="damage"></param>
        /// <param name="stansMinus"></param>
        public abstract void GotAttack(string attackMode, float damage, float stansMinus);

        public abstract void GotAirbornAttack(string attackMode, float damage, float stansMinus, float airBoneValue);

        protected abstract void attack_Logic(float dmg);
        

        protected virtual void HpUpdate()
        {
            if (currentHp <= 0f)
            {
                Dead();
            }
        }

        public virtual void Dead()
        {
            if (wave)
            {
                wave.monsterList.Remove(this);
                wave.Check();
            }
        }

        protected IEnumerator Invincibility(float time)
        {
            youCantHurtMe = true;
            yield return new WaitForSeconds(time);
            youCantHurtMe = false;
        }
        public void DefaultAttack()
        {
            CurrentAttack = Attack();
            StartCoroutine(CurrentAttack);
        }

        protected abstract IEnumerator Attack();
    }
}
