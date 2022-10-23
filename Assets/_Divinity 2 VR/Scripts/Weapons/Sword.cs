using HurricaneVR.Framework.Core;
using HurricaneVR.Framework.Core.Utils;
using UnityEngine;

namespace intheclouds
{
    public class Sword : MonoBehaviour
    {
        public Collider hitEnemyCollider;
        public AudioClip enemyHitClip;
        public float hitCooldown = 0.25f;
        public float lowSpeedHitEnemy = 5f;
        public float medSpeedHitEnemy = 10f;
        public float fastSpeedHitEnemy = 15f;
        public int requiredAP = 2;
        public int baseDamage = 1;
        public float criticalDamageMultiplier = 1.8f;
        public PlayerStats combatant;
        private HVRGrabbable grabbable;
        private bool inEnemyCollider;
        private EnemyStats currentEnemyStats;

        private void Start()
        {
            grabbable = GetComponent<HVRGrabbable>();
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                if (combatant == null || combatant.LocalUserObjects.spiritWander.isActivated || !combatant.Turn && combatant.InCombat)
                {
                    return;
                }

                if (combatant.CurrentAP >= requiredAP && collision.relativeVelocity.magnitude > lowSpeedHitEnemy)
                {
                    currentEnemyStats = collision.gameObject.GetComponentInParent<EnemyStats>();
                    SFXPlayer.Instance.PlaySFXRandomPitchAttach(enemyHitClip, transform, 0.9f, 1.1f, 0.5f, 20);

                    if (!currentEnemyStats.isAlive)
                    {
                        return;
                    }

                    var totalDamage = (int) (baseDamage * (combatant.Strength * 1.05f));
                    currentEnemyStats.TakeDamage(combatant, Helpers.CalculateDamageRange(totalDamage, combatant, criticalDamageMultiplier),
                        DamageType.Physical, ElementalType.None, null);

                    if (combatant.InCombat)
                    {
                        combatant.UseAP(requiredAP);
                    }
                    
                    hitEnemyCollider.gameObject.SetActive(false);
                    Invoke(nameof(ResetCollision), hitCooldown);
                }
            }
        }

        private void ResetCollision()
        {
            hitEnemyCollider.gameObject.SetActive(true);
        }

        public void UpdateWielder()
        {
            combatant = grabbable.PrimaryGrabber.transform.root.GetComponent<LocalUserObjects>().PlayerStats;
            GetComponent<WeaponSwipeSFX>().wielderCharacterController =
                combatant.LocalUserObjects.HVRPlayerController.GetComponent<CharacterController>();
        }
    }
}