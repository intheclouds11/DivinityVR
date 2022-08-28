using System.Collections;
using System.Collections.Generic;
using HurricaneVR.Framework.Core;
using HurricaneVR.Framework.Core.Grabbers;
using HurricaneVR.Framework.Weapons.Bow;
using UnityEngine;

namespace intheclouds
{
    public class ITCArrow : HVRArrow
    {
        public int damage = 10;
        public float criticalMultiplier = 1.5f;
        public PlayerStats wieldingUser;

        protected override void OnGrabbed(HVRGrabberBase arg0, HVRGrabbable arg1)
        {
            base.OnGrabbed(arg0, arg1);
            wieldingUser = Grabbable.PrimaryGrabber.transform.root.GetComponent<LocalUserObjects>().PlayerStats;
        }

        protected override void OnCollisionEnter(Collision collision)
        {
            if (!enabled) return;
            if (Rigidbody.velocity.magnitude > 1)
            {
                if (collision.gameObject.CompareTag("EnemyHead"))
                {
                    var actualDamage = Random.Range(damage - (int) (damage * 0.1f), damage + (int) (damage * 0.1f)) * criticalMultiplier;
                    collision.gameObject.GetComponentInParent<EnemyStats>()?.TakeDamage(wieldingUser, DamageType.Physical, (int) actualDamage);
                    enabled = false;
                }
                else if (collision.gameObject.CompareTag("EnemyBody"))
                {
                    var actualDamage = Random.Range(damage - (int) (damage * 0.1f), damage + (int) (damage * 0.1f));
                    collision.gameObject.GetComponentInParent<EnemyStats>()?.TakeDamage(wieldingUser, DamageType.Physical, actualDamage);
                    enabled = false;
                }
            }

            base.OnCollisionEnter(collision);
        }
    }
}