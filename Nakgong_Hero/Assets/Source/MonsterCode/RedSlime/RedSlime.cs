using System.Collections;
using Source.Item;
using Source.PlayerCode;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Source.MonsterCode.RedSlime
{
    public class RedSlime : DefaultCommonMonster
    {
        [SerializeField] private float attackDelay;
        [SerializeField] private float recoveryDelay;
        [SerializeField] private Vector2 attackBoxSize;
        private int _nextMove;
        public readonly int IsWalking = Animator.StringToHash("isWalking");
        public IEnumerator CurrentThink;
        private bool isStunned = false;
        [SerializeField] private bool onAttackPrefer;

        public override void Start()
        {
            base.Start();
            ThinkFunc(5f);
        }
        public override void GotAttack(string attackMode, float gotDamage, float stansMinus)
        {
            if (youCantHurtMe)
            {
                return;
            }
        
            switch (attackMode)
            {
                case "Default":
                    float realDamage = gotDamage + InvManager.instance.airBonePower/100f;
                    attack_Logic(realDamage);
                    if (!isAlive) return;
                    KnockBack(stansMinus);
                    StartCoroutine(Invincibility(0.1f));
                    return;
                case "Throw":
                    attack_Logic(gotDamage);
                    if (!isAlive) return;
                    KnockBack(stansMinus);
                    StartCoroutine(Invincibility(0.3f));
                    break;
            }
        }

        public override void GotAirbornAttack(string attackMode, float gotDamage, float stansMinus, float airBoneValue)
        {
            if (youCantHurtMe)
            {
                return;
            }
            switch (attackMode)
            {
                case "대지분쇄":
                    attack_Logic(gotDamage);
                    AirBone(stansMinus, airBoneValue);
                    StartCoroutine(Invincibility(0.1f));
                    break;
            }
        }

        protected override IEnumerator Attack()
        {
            anim.Play("공격");
            isAttacking = true;
            if (PlayerController.instance.playerPos.x - transform.position.x > 0)
            {
                watchingLeft = false;
                transform.localScale = new Vector3(-1 * localScaleTemp.x, transform.localScale.y);
            }
            else
            {
                watchingLeft = true;
                transform.localScale = new Vector3(localScaleTemp.x, transform.localScale.y);
            }
            _nextMove = 0;
            thisRigidbody2D.linearVelocity = Vector2.zero;
            yield return new WaitForSeconds(attackDelay);
            var ray = Physics2D.BoxCastAll(transform.position, new Vector2(0.01f, attackBoxSize.y), 0, 
                watchingLeft ? Vector2.left : Vector2.right, attackBoxSize.x, LayerMask.GetMask("Player"));
            foreach(var o in ray)
            {
                if(o.transform.CompareTag("Player"))
                    PlayerController.instance.GotAttack(monsterData.Damage, stunTime: monsterData.StunPower);
            }
            yield return new WaitForSeconds(recoveryDelay);
            isAttacking = false;
        }

        private void KnockBack(float stanceMinus)
        {
            stans -= stanceMinus;
            if (stans > 0f) return;
            if(CurrentAttack is not null)
                StopCoroutine(CurrentAttack);
            anim.StopPlayback();
            GameUtil.instance.CoolBool(recoveryDelay,v=>isStunned = v, false);
            isAttacking = false;
            thisRigidbody2D.AddForce(gameObject.transform.position.x - PlayerController.instance.playerPos.x > 0
                ? new Vector2(1f * knockbackForce - stans, InvManager.instance.airBonePower)
                : new Vector2(-1f * knockbackForce, InvManager.instance.airBonePower));
            stans = stansTemp;
        }
        public void AirBone(float stansMinus, float airBoneValue)
        {
            stans -= stansMinus;
            if (stans > 0f) return;
            thisRigidbody2D.AddForce(new Vector2(gameObject.transform.position.x - PlayerController.instance.playerPos.x > 0 ? 0.3f:-0.3f, airBoneValue));
            stans =stansTemp;
        }
        public void FixedUpdate()
        {
            if (isStunned) return;
            thisRigidbody2D.linearVelocity = new Vector2(_nextMove * currentSpeed, thisRigidbody2D.linearVelocity.y);
            if (!isAggroling)
            {
                if (math.abs(PlayerController.instance.playerPos.x - transform.position.x) < aggroRange.x)
                {
                    if (math.abs(PlayerController.instance.playerPos.y - transform.position.y) < aggroRange.y)
                    {
                        isAggroling = true;
                        StartCoroutine(Aggro());
                        StopCoroutine(CurrentThink);
                    }
                }
                Vector2 front = new Vector2(thisRigidbody2D.position.x + _nextMove, thisRigidbody2D.position.y);
                RaycastHit2D raycast = Physics2D.Raycast(front, Vector3.down,1,LayerMask.GetMask("Default"));
                if (raycast.collider is not null) return;
                StopCoroutine(CurrentThink);
                _nextMove *= -1;
            }
        }

        public IEnumerator Aggro()
        {
            StopCoroutine(CurrentThink);
            anim.StopPlayback();
            currentSpeed += aggroSpeed;
            while (math.abs(PlayerController.instance.playerPos.x - transform.position.x) < aggroRange.x && math.abs(PlayerController.instance.playerPos.y - transform.position.y) < aggroRange.y)
            {

                if (isStunned) yield return null;
                if (!isAttacking)
                {
                    if (math.abs(PlayerController.instance.playerPos.x - transform.position.x) > attackRange)
                    {
                        thisRigidbody2D.linearVelocity = new Vector2(_nextMove * currentSpeed, thisRigidbody2D.linearVelocity.y);
                        anim.SetBool(IsWalking, true);
                        if (PlayerController.instance.playerPos.x - transform.position.x > 0)
                        {
                            watchingLeft = false;
                            _nextMove = 1;
                            transform.localScale = new Vector3(-1 * localScaleTemp.x, transform.localScale.y);
                        }
                        else
                        {
                            watchingLeft = true;
                            _nextMove = -1;
                            transform.localScale = new Vector3(localScaleTemp.x, transform.localScale.y);
                        }
                    }
                    else
                    {
                        _nextMove = 0;
                        anim.SetBool(IsWalking, false);
                        DefaultAttack();
                    }
                }
                else
                {
                    while (isAttacking)
                    {
                        yield return null;
                    }
                }
                if (!isAlive)
                {
                    yield break;
                }
                yield return null;
            }
            currentSpeed -= aggroSpeed;
            isAggroling = false;
            ThinkFunc(5f);
        }

        private IEnumerator Think(float nextTime)
        {
            while(isStunned) yield return null;
            currentSpeed = monsterData.Speed;
            _nextMove = Random.Range(-1, 2);
            
            if (_nextMove == 1)
            {
                watchingLeft = false;
                transform.localScale = new Vector3(-1*localScaleTemp.x, transform.localScale.y);
                anim.SetBool(IsWalking,true);
            }
            else if (_nextMove == -1)
            {
                watchingLeft = true;
                transform.localScale = new Vector3(localScaleTemp.x, transform.localScale.y);
                anim.SetBool(IsWalking,true);
            }
            else
            {
                anim.SetBool(IsWalking,false);
            }
            yield return new WaitForSeconds(nextTime);
            if (!isAlive)
            {
                yield break;
            }
            ThinkFunc(nextTime);
        }

        private void ThinkFunc(float nextTime)
        {
            CurrentThink = Think(nextTime);
            StartCoroutine(CurrentThink);
        }
    }
}