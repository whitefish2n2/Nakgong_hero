using System;
using System.Collections;
using Source.Item;
using Source.MonsterCode;
using UnityEngine;

namespace Source.PlayerCode
{
    public class Deager : MonoBehaviour
    {
        [SerializeField] private float ThrowingSpeed;
        [SerializeField] private float TurnBackSpeed;
        [SerializeField] private float AirWaitTime;
        [SerializeField] private Rigidbody2D rigid;
        public static bool isCrashWithWall = false;
        public bool _isThrowing;
        private Vector3 _throwPosTemp;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        public void ThrowAt_withThrowRange(Vector3 ThrowPos, float Range)
        {
            bool isRight;
            if (transform.position.x - ThrowPos.x > 0)
            {
                //왼쪽으로 던지는 애니메이션 재생
                isRight = false;
            }
            else
            {
                // ''
                isRight = true;
            }
            StartCoroutine(ThrowAt(ThrowPos, Range, isRight));
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_isThrowing)
            {
                if (other.CompareTag("DefaultMonster"))
                {
                    other.GetComponent<DefaultMonster>().GotAttack("Throw", InvManager.instance.GetAttackPower() * 1371, InvManager.instance.stansBreak*40);
                }
            }
        
        }

        private IEnumerator ThrowAt(Vector3 throwPosIE, float rangeIE, bool toRight)
        {
            gameObject.transform.parent = null;
            _throwPosTemp = throwPosIE;
            Vector3 playerPos = PlayerController.instance.playerPos;
            Vector2 dirvec = throwPosIE - playerPos;
            float mouseRange = Vector2.Distance(throwPosIE, playerPos);
            _isThrowing = true;
            gameObject.transform.localScale = new Vector3(toRight ? -1:1,gameObject.transform.localScale.y,gameObject.transform.localScale.z);
            gameObject.transform.up = dirvec.normalized;
            while (Vector2.Distance(transform.position,playerPos) < rangeIE && Vector2.Distance(transform.position,playerPos) < mouseRange)
            {
                if (!isCrashWithWall && PlayerController.instance.isThrowing && !PlayerController.instance.isGetHooking)
                {
                    rigid.linearVelocity = dirvec * ThrowingSpeed;
                    yield return null;
                }
                else
                {
                    break;
                }
            }
            rigid.linearVelocity -= dirvec * ThrowingSpeed;
            StartCoroutine(TurnBack());
        }

        private IEnumerator TurnBack()
        {
            GroundDeagerCheck.dontCheck = true;
            isCrashWithWall = false;
            yield return new WaitForSeconds(AirWaitTime);
            var turnbackTemp = TurnBackSpeed;
            while (Vector2.Distance(transform.position, PlayerController.instance.playerPos) > 0.9f)
            {
                Vector2 dirvec = _throwPosTemp - transform.position;
                transform.position = Vector2.Lerp(transform.position, PlayerController.instance.playerPos, TurnBackSpeed * Time.deltaTime);
                transform.up = dirvec;
                TurnBackSpeed += 3f * Time.deltaTime;
                yield return null;
            }
            TurnBackSpeed = turnbackTemp;
            PlayerController.instance.isGetHooking = false;
            PlayerController.instance.isThrowing = false;
            GroundDeagerCheck.dontCheck = false;
            gameObject.transform.position =
                new Vector3(PlayerController.instance.playerPos.x,
                    PlayerController.instance.playerPos.y + 0.26f, 0f);
            _isThrowing = false;
            PlayerController.instance.ChangeDeagerMode(PlayerController.SwordMode.Hold);
            gameObject.SetActive(false);
        }

        public void InstanceTurnBack()
        {
            GroundDeagerCheck.dontCheck = true;
            isCrashWithWall = false;
            PlayerController.instance.isGetHooking = false;
            PlayerController.instance.isThrowing = false;
            GroundDeagerCheck.dontCheck = false;
            _isThrowing = false;
            PlayerController.instance.ChangeDeagerMode(PlayerController.SwordMode.Hold);
        }
        

        /*
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ground") && !dontCheck)
        {
            isCrashWithWall = true;
        }
    }
    */
    }
}
