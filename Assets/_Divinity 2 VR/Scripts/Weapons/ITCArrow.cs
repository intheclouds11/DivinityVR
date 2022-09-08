using System.Collections;
using System.Collections.Generic;
using HurricaneVR.Framework.Core;
using HurricaneVR.Framework.Core.Grabbers;
using HurricaneVR.Framework.Core.Stabbing;
using HurricaneVR.Framework.Core.Utils;
using HurricaneVR.Framework.Weapons.Bow;
using UnityEngine;

namespace intheclouds
{
    public class ITCArrow : HVRArrow
    {
        public int requiredAP = 2;
        public int damage = 10;
        public float criticalMultiplier = 1.5f;
        public PlayerStats wieldingUser;
        public AudioClip damageAudioClip;
        public AudioClip noDamageAudioClip;

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
                    if (!CheckIfCanDamage()) return;
                    var actualDamage = Random.Range(damage - (int) (damage * 0.1f), damage + (int) (damage * 0.1f)) * criticalMultiplier;
                    collision.gameObject.GetComponentInParent<EnemyStats>()?.TakeDamage(wieldingUser, DamageType.Physical, (int) actualDamage);
                }
                else if (collision.gameObject.CompareTag("EnemyBody"))
                {
                    if (!CheckIfCanDamage()) return;
                    var actualDamage = Random.Range(damage - (int) (damage * 0.1f), damage + (int) (damage * 0.1f));
                    collision.gameObject.GetComponentInParent<EnemyStats>()?.TakeDamage(wieldingUser, DamageType.Physical, actualDamage);
                }

                if (collision.gameObject.CompareTag("EnemyBody") || collision.gameObject.CompareTag("EnemyHead"))
                {
                    wieldingUser.UseAP(requiredAP);
                    SFXPlayer.Instance.PlaySFXRandomPitchAttach(damageAudioClip, transform, 1f, 1.1f, 0.5f, 20);
                    enabled = false;
                }
            }

            base.OnCollisionEnter(collision);
        }

        // arrow does no damage during combat if not player's turn
        private bool CheckIfCanDamage()
        {
            if (!wieldingUser.LocalUserObjects.spiritWander.activated && (!wieldingUser.InCombat || (wieldingUser.CurrentAP >= 2 && wieldingUser.Turn)))
            {
                return true;
            }

            SFXPlayer.Instance.PlaySFXRandomPitchAttach(noDamageAudioClip, GameManager.Instance.FindControlledPlayer().gameObject.transform, 1f, 1.05f, 0.7f, 20);
            Destroy(gameObject);
            return false;
        }
    }
}