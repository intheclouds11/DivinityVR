using System;
using HurricaneVR.Framework.Components;
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
        public int baseDamage = 10;
        public float criticalDamageMultiplier = 1.8f;
        private PlayerStats wieldingUser;
        private bool inEnemyCollider;
        private EnemyStats currentEnemyStats;
        public event Action SwordAppliedDamage;

        private void OnCollisionEnter(Collision collision)
        {
            var relativeVelocity = collision.relativeVelocity.magnitude;
            var objectDamageHandler = collision.collider.GetComponent<HVRDamageHandlerBase>();
            if (objectDamageHandler)
            {
                objectDamageHandler.TakeDamage(Mathf.Clamp(relativeVelocity, 0, 10));
            }
            
            if (wieldingUser == null)
            {
                return;
            }
            
            if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                if (!wieldingUser.CanPerformActions())
                {
                    return;
                }

                if (wieldingUser.CurrentAP >= requiredAP && relativeVelocity > lowSpeedHitEnemy)
                {
                    currentEnemyStats = collision.gameObject.GetComponentInParent<EnemyStats>();
                    SFXPlayer.Instance.PlaySFXRandomPitchAttach(enemyHitClip, transform, 0.9f, 1.1f, 0.5f, 20);

                    if (!currentEnemyStats.isAlive)
                    {
                        return;
                    }

                    var totalDamage = (int) Math.Ceiling(baseDamage * (wieldingUser.Strength * 0.105f)); 
                    // 0.105 comes from dividing base strength (10) by 10 and multiplying 1.05 (5%+). every strength point is 5% damage boost
                    currentEnemyStats.TakeDamage(wieldingUser, Helpers.CalculateDamageRange(totalDamage, wieldingUser, criticalDamageMultiplier),
                        DamageType.Physical, ElementalType.None, null);

                    if (wieldingUser.InCombat)
                    {
                        wieldingUser.UseAP(requiredAP);
                    }
                    
                    hitEnemyCollider.gameObject.SetActive(false);
                    SwordAppliedDamage?.Invoke();
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
            wieldingUser = LocalUserObjects.Instance.PlayerStats;
            GetComponent<WeaponSwipeSFX>().wielderCharacterController = LocalUserObjects.Instance.HVRPlayerController.GetComponent<CharacterController>();
        }
    }
}