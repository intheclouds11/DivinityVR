using HurricaneVR.Framework.Core;
using HurricaneVR.Framework.Core.Grabbers;
using HurricaneVR.Framework.Core.Utils;
using HurricaneVR.Framework.Weapons.Bow;
using UnityEngine;

namespace intheclouds
{
    public class ITCArrow : HVRArrow
    {
        public int requiredAP = 2;
        public int baseDamage = 1;
        public float criticalDamageMultiplier = 1.5f;
        public ElementalType elementalType = ElementalType.None;
        public DamageType damageType = DamageType.Physical;
        public StatusEffect statusEffect;
        public PlayerStats player;
        public AudioClip damageAudioClip;

        protected override void OnGrabbed(HVRGrabberBase arg0, HVRGrabbable arg1)
        {
            base.OnGrabbed(arg0, arg1);
            player = Grabbable.PrimaryGrabber.transform.GetComponentInParent<LocalUserObjects>().PlayerStats;
        }

        protected override void OnCollisionEnter(Collision collision)
        {
            if (!enabled) return;

            if (Rigidbody.velocity.magnitude > 1)
            {
                ProcessHitEnemy(collision);
                ProcessHitPlayer(collision);
            }

            base.OnCollisionEnter(collision);
            enabled = false;
        }

        private void ProcessHitEnemy(Collision collision)
        {
            if (collision.gameObject.CompareTag("EnemyHead"))
            {
                var totalDamage = (int) (baseDamage * criticalDamageMultiplier * (player.Finesse * 1.05));
                collision.gameObject.GetComponentInParent<EnemyStats>()?.TakeDamage(player,
                    Helpers.CalculateDamageRange(totalDamage, player), damageType, elementalType, statusEffect);
            }
            else if (collision.gameObject.CompareTag("EnemyBody"))
            {
                var totalDamage = (int) (baseDamage * (player.Finesse * 1.05));
                collision.gameObject.GetComponentInParent<EnemyStats>()?.TakeDamage(player,
                    Helpers.CalculateDamageRange(totalDamage, player, criticalDamageMultiplier), damageType, elementalType, statusEffect);
            }

            if (collision.gameObject.CompareTag("EnemyBody") || collision.gameObject.CompareTag("EnemyHead"))
            {
                SFXPlayer.Instance.PlaySFXRandomPitchAttach(damageAudioClip, transform, 1f, 1.1f, 0.5f, 20);
            }
        }

        private void ProcessHitPlayer(Collision collision)
        {
            if (collision.gameObject.CompareTag("Player") || collision.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                // damage player
            }
        }

        // Instead.. Preventing arrow nock if not players turn or not enough AP
        // // arrow does no damage during combat if not player's turn
        // private bool CheckIfCanDamage()
        // {
        //     if (!combatant.LocalUserObjects.spiritWander.activated && (!combatant.InCombat || (combatant.CurrentAP >= 2 && combatant.Turn)))
        //     {
        //         return true;
        //     }
        //
        //     SFXPlayer.Instance.PlaySFXRandomPitchAttach(noDamageAudioClip, GameManager.Instance.FindControlledPlayer().gameObject.transform, 1f,
        //         1.05f, 0.7f, 20);
        //     Destroy(gameObject);
        //     return false;
        // }
    }
}