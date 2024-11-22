using Source.Item;
using Source.MonsterCode;
using UnityEngine;

namespace Source.PlayerCode
{
    public class HitBox : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("DefaultMonster"))
            {
                other.gameObject.GetComponent<DefaultMonster>()
                    .GotAttack(PlayerController.instance.attackMode, InvManager.instance.GetAttackPower(), InvManager.instance.stansBreak);
            }
            else if (other.gameObject.CompareTag("MobGenerator"))
            {
                other.GetComponent<global::MobGenerator>()?.Trigger();
            }
        }
    }
}
