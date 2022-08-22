using System.Collections;
using System.Collections.Generic;
using HurricaneVR.Framework.Weapons.Bow;
using UnityEngine;

namespace intheclouds
{
    public class ITCArrow : HVRArrow
    {
        public int damage = 10;
        public int criticalMultiplier = 2;

        protected override void OnCollisionEnter(Collision collision)
        {
            if (!enabled) return;
            if (Rigidbody.velocity.magnitude > 1)
            {
                Debug.Log($"arrow hit: {collision}", collision.gameObject);
                if (collision.gameObject.CompareTag("EnemyHead"))
                {
                    Debug.Log("arrow hit head");
                    var actualDamage = Random.Range(damage - (int) (damage * 0.1f), damage + (int) (damage * 0.1f));
                    collision.gameObject.GetComponentInParent<EnemyStats>()?.TakeDamage(DamageType.Physical, actualDamage * criticalMultiplier);
                    enabled = false;
                }
                else if (collision.gameObject.CompareTag("EnemyBody"))
                {
                    Debug.Log("arrow hit body");

                    var actualDamage = Random.Range(damage - (int) (damage * 0.1f), damage + (int) (damage * 0.1f));
                    collision.gameObject.GetComponentInParent<EnemyStats>()?.TakeDamage(DamageType.Physical, actualDamage);
                    enabled = false;
                }
            }
            base.OnCollisionEnter(collision);
        }
    }
}