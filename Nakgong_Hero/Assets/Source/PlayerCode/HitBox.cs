using System;
using System.Collections;
using System.Collections.Generic;
using Source.MonsterCode;
using UnityEngine;

public class HitBox : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("DefaultMonster"))
        {
            other.gameObject.GetComponent<DefaultMonster>()
                .GotAttack(PlayerController.Instance.attackMode, InvManager.Instance.AttackPower, InvManager.Instance.stans);
        }
        else if (other.gameObject.CompareTag("MobGenerator"))
        {
            other.GetComponent<MobGenerator>().Trigger();
        }
    }
}
