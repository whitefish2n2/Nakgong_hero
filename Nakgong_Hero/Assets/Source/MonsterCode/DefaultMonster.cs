using System.Collections;
using Source.Item;
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
        public virtual void Awake()
        {
            thisRigidbody2D = gameObject.GetComponent<Rigidbody2D>();
            gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>();
            currentHp = monsterData.MaxHp;
            currentHp *= InvManager.instance.difficulty;
            currentSpeed = monsterData.Speed;
            localScaleTemp = transform.localScale;
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

        protected abstract void attack_Effect(float dmg);
        

        protected virtual void HpUpdate()
        {
            if (currentHp <= 0f)
            {
                Dead();
            }
        }

        public abstract void Dead();

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
