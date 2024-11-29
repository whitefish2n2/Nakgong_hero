using System;
using Source.PlayerCode;
using UnityEngine;

namespace Source.MonsterCode.TheEye
{
    public class Razer : MonoBehaviour
    {
        [SerializeField] private bool canHurt = false;
        private void OnTriggerStay2D(Collider2D other)
        {
            if (!canHurt) return;
            if (other.CompareTag("Player"))
            {
                PlayerController.instance.GotAttack(TheEyeCode.instance.monsterData.Damage, invincibleTime:0.1f);
            }
        }
    }
}
