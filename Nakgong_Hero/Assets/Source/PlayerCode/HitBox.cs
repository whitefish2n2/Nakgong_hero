using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBox : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("DefaultMonster"))
        {
            other.gameObject.GetComponent<DefaultMonster>()
                .gotattack(PlayerController.AttackMode, PlayerController.AttackPower, PlayerController.stans);
        }
    }
}
