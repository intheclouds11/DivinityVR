using System;
using HurricaneVR.Framework.Core;
using HurricaneVR.Framework.Core.Utils;
using intheclouds;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

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
        public int physicalDamage = 10;
        public int magicDamage = 0;
        public PlayerStats wieldingUser;
        private HVRGrabbable grabbable;
        private bool inEnemyCollider;
        // private Collision currentEnemyCollision;
        private EnemyStats currentEnemyStats;
        // private bool canDamage = true;

        private void Start()
        {
            grabbable = GetComponent<HVRGrabbable>();
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                if (wieldingUser == null || wieldingUser.LocalUserObjects.spiritWander.activated || !wieldingUser.Turn && !wieldingUser.ExplorationMode)
                {
                    return;
                }

                if (wieldingUser.CurrentAP >= requiredAP && collision.relativeVelocity.magnitude > lowSpeedHitEnemy)
                {
                    currentEnemyStats = collision.gameObject.GetComponentInParent<EnemyStats>();
                    SFXPlayer.Instance.PlaySFXRandomPitchAttach(enemyHitClip, transform, 0.9f, 1.1f, 0.5f, 20);

                    if (!currentEnemyStats.isAlive)
                    {
                        return;
                    }

                    if (physicalDamage > 0)
                    {
                        var actualDamage = Random.Range(physicalDamage - (int) (physicalDamage * 0.1f),
                            physicalDamage + (int) (physicalDamage * 0.1f));
                        currentEnemyStats.TakeDamage(wieldingUser, DamageType.Physical, actualDamage);
                    }
                    else if (magicDamage > 0)
                    {
                        var actualDamage = Random.Range(magicDamage - (int) (magicDamage * 0.1f),
                            magicDamage + (int) (magicDamage * 0.1f));
                        currentEnemyStats.TakeDamage(wieldingUser, DamageType.Magic, actualDamage);
                    }

                    if (!wieldingUser.ExplorationMode)
                    {
                        wieldingUser.UseAP(requiredAP);
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
            wieldingUser = grabbable.PrimaryGrabber.transform.root.GetComponent<LocalUserObjects>().PlayerStats;
            GetComponent<WeaponSwipeSFX>().wielderCharacterController = wieldingUser.LocalUserObjects.HVRPlayerController.GetComponent<CharacterController>();
        }
    }
}