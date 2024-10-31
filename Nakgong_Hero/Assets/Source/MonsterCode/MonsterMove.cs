using System.Collections;
using Source.PlayerCode;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Source.MonsterCode
{
    public class MonsterMove : MonoBehaviour
    {
        [SerializeField] private float speed;
        [SerializeField] private Animator anim;
        [SerializeField] private Vector2 aggroRange;
        [SerializeField] private float aggroSpeed;
        [SerializeField] private Vector2 attackBoxSize;
        [SerializeField] private float attackSpeed; 
        public float damage;
        public float stunTime;
        private Vector2 _localScale;
        [SerializeField]private Rigidbody2D rigid;
        private bool _isAggroling;
        private bool _watchingLeft = true;
        private int _nextMove;
        public bool isAttacking;
        private readonly int _isWalking = Animator.StringToHash("isWalking");
        public DefaultMonster monster;

        public void Start()
        {
            _localScale = transform.localScale;
            //rigid = GetComponent<Rigidbody2D>();
            isAttacking = false;
            Invoke(nameof(Think),5f);
        }

        public void FixedUpdate()
        {
            rigid.linearVelocity = new Vector2(_nextMove * speed, rigid.linearVelocity.y);
            if (!_isAggroling)
            {
                if (math.abs(PlayerController.Instance.playerPos.x - transform.position.x) < aggroRange.x)
                {
                    if (math.abs(PlayerController.Instance.playerPos.y - transform.position.y) < aggroRange.y)
                    {
                        _isAggroling = true;
                        StartCoroutine(nameof(Aggro));
                    }
                }
                Vector2 front = new Vector2(rigid.position.x + _nextMove, rigid.position.y);
                Debug.DrawRay(front, Vector3.down, new Color(1,0,0));
                RaycastHit2D raycast = Physics2D.Raycast(front, Vector3.down,1,LayerMask.GetMask("Default"));
                if (raycast.collider is not null) return;
                _nextMove *= -1;
                CancelInvoke();
                Invoke(nameof(Think),2); 
                speed = 0.2f;
            }
        }

        public IEnumerator Aggro()
        {
            anim.StopPlayback();
            speed += aggroSpeed;
            CancelInvoke(nameof(Think));
            while (math.abs(PlayerController.Instance.playerPos.x - transform.position.x) < aggroRange.x && math.abs(PlayerController.Instance.playerPos.y - transform.position.y) < aggroRange.y)
            {
                if (!isAttacking)
                {
                    if (math.abs(PlayerController.Instance.playerPos.x - transform.position.x) > 0.5f)
                    {
                        rigid.linearVelocity = new Vector2(_nextMove * speed, rigid.linearVelocity.y);
                        anim.SetBool(_isWalking, true);
                        if (PlayerController.Instance.playerPos.x - transform.position.x > 0)
                        {
                            _watchingLeft = false;
                            _nextMove = 1;
                            transform.localScale = new Vector3(-1 * _localScale.x, transform.localScale.y);
                        }
                        else
                        {
                            _watchingLeft = true;
                            _nextMove = -1;
                            transform.localScale = new Vector3(_localScale.x, transform.localScale.y);
                        }
                    }
                    else
                    {
                        _nextMove = 0;
                        anim.SetBool(_isWalking, false);
                        StartCoroutine(Attack());
                    }
                }
                yield return null;
            }

            speed -= aggroSpeed;
            _isAggroling = false;
            Invoke(nameof(Think),2f);
        }

        public void Think()
        {
            speed = 1f;
            _nextMove = Random.Range(-1, 2);
            Invoke(nameof(Think),5f);
            if (_nextMove == 1)
            {
                _watchingLeft = false;
                transform.localScale = new Vector3(-1*_localScale.x, transform.localScale.y);
                anim.SetBool(_isWalking,true);
            }
            else if (_nextMove == -1)
            {
                _watchingLeft = true;
                transform.localScale = new Vector3(_localScale.x, transform.localScale.y);
                anim.SetBool(_isWalking,true);
            }
            else
            {
                anim.SetBool(_isWalking,false);
            }
        }

        private IEnumerator Attack()
        {
            isAttacking = true;
            yield return new WaitForSeconds(attackSpeed);
            Debug.Log("공격하다");
            RaycastHit2D[] ray = { };
            Physics2D.BoxCastNonAlloc(transform.position, new Vector2(0.01f, attackBoxSize.y), 0, 
                _watchingLeft ? Vector2.left : Vector2.right, ray, attackBoxSize.x, LayerMask.GetMask("Player"));
        
            foreach(var o in ray)
            {
                if(o.transform.CompareTag("Player"))
                    PlayerController.GotAttack(damage, stunTime);
            }
            isAttacking = false;
        }
    }
}
