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
        private bool _isThrowing;
        private Vector3 _throwPosTemp;
        public void ThrowAt_withThrowRange(Vector3 ThrowPos, float Range)
        {
            StartCoroutine(ThrowAt(ThrowPos, Range));
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

        private IEnumerator ThrowAt(Vector3 throwPosIE, float rangeIE)
        {
            gameObject.transform.parent = null;
            _throwPosTemp = throwPosIE;
            Vector3 playerPos = PlayerController.Instance.playerPos;
            Vector2 dirvec = throwPosIE - playerPos;
            float mouseRange = Vector2.Distance(throwPosIE, playerPos);
            _isThrowing = true;
            gameObject.transform.up = dirvec.normalized;
            while (Vector2.Distance(transform.position,playerPos) < rangeIE && Vector2.Distance(transform.position,playerPos) < mouseRange)
            {
                if (!isCrashWithWall && PlayerController.Instance.isThrowing && !PlayerController.Instance.isGetHooking)
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
            while (Vector2.Distance(transform.position, PlayerController.Instance.playerPos) > 0.9f)
            {
                Vector2 dirvec = _throwPosTemp - transform.position;
                transform.position = Vector2.Lerp(transform.position, PlayerController.Instance.playerPos, TurnBackSpeed * Time.deltaTime);
                transform.up = dirvec;
                TurnBackSpeed += 3f * Time.deltaTime;
                yield return null;
            }
            TurnBackSpeed = turnbackTemp;
            PlayerController.Instance.isGetHooking = false;
            PlayerController.Instance.isThrowing = false;
            GroundDeagerCheck.dontCheck = false;
            gameObject.transform.parent = PlayerController.Instance.gameObject.transform;
            gameObject.transform.position =
                new Vector3(PlayerController.Instance.playerPos.x,
                    PlayerController.Instance.playerPos.y + 0.26f, 0f);
            _isThrowing = false;
        }

        public void InstanceTurnBack()
        {
            GroundDeagerCheck.dontCheck = true;
            isCrashWithWall = false;
            PlayerController.Instance.isGetHooking = false;
            PlayerController.Instance.isThrowing = false;
            GroundDeagerCheck.dontCheck = false;
            gameObject.transform.parent = PlayerController.Instance.gameObject.transform;
            gameObject.transform.position =
                new Vector3(PlayerController.Instance.playerPos.x,
                    PlayerController.Instance.playerPos.y + 0.26f, 0f);
            _isThrowing = false;
        }

        private void Update()
        {
            if (!PlayerController.Instance.isThrowing)
            {
                float zRotPlus = (PlayerController.Instance.playerRotate.y <= 0.5f) ? -1f : 1f;
                gameObject.transform.localPosition = new Vector3(0,0.26f, 0f);
                gameObject.transform.rotation = Quaternion.Euler(0f, 0f, zRotPlus*45);
                gameObject.transform.localScale = new Vector3(-zRotPlus, 1f, 1f);
            }
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
