using System;
using HurricaneVR.Framework.Core;
using intheclouds;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace intheclouds
{
    public class Sword : MonoBehaviour
    {
        public float hitCooldown = 0.25f;
        public float requiredHitSpeed = 1f;
        public int requiredAP = 2;
        public int physicalDamage = 10;
        public int magicDamage = 0;
        private PlayerStats wieldingUser;
        private Rigidbody rb;
        private HVRGrabbable grabbable;
        public float hitCooldownTimer;
        private bool enemyHit;
        private bool inEnemyCollider;
        private Collision currentEnemyCollision;
        private EnemyStats currentEnemyStats;

        private void Start()
        {
            grabbable = GetComponent<HVRGrabbable>();
            rb = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            if (wieldingUser == null) return;

            if (wieldingUser.explorationMode) return;

            if (wieldingUser.playerTurnCombat && wieldingUser.currentAP >= requiredAP)
            {
                if (enemyHit && !inEnemyCollider && hitCooldownTimer > 0)
                {
                    hitCooldownTimer -= Time.deltaTime;
                }

                else if (currentEnemyCollision != null && !inEnemyCollider && hitCooldownTimer <= 0)
                {
                    enemyHit = false;
                    currentEnemyCollision.gameObject.layer = LayerMask.NameToLayer("Enemy");
                    foreach (Transform child in currentEnemyCollision.gameObject.transform)
                    {
                        child.gameObject.layer = LayerMask.NameToLayer("Enemy");
                    }
                }
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (hitCooldownTimer > 0 || wieldingUser == null || !wieldingUser.playerTurnCombat) return;

            if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                if (inEnemyCollider && enemyHit) return;

                if (rb.velocity.magnitude > requiredHitSpeed)
                {
                    currentEnemyCollision = collision;
                    inEnemyCollider = true;

                    if (wieldingUser.currentAP > requiredAP)
                    {
                        currentEnemyStats = collision.gameObject.GetComponentInParent<EnemyStats>();
                        currentEnemyStats.EnemyDied += OnEnemyDead;

                        if (physicalDamage > 0)
                        {
                            var actualDamage = Random.Range(physicalDamage - (int) (physicalDamage * 0.1f),
                                physicalDamage + (int) (physicalDamage * 0.1f));
                            currentEnemyStats.TakeDamage(DamageType.Physical, actualDamage);
                        }
                        else if (magicDamage > 0)
                        {
                            var actualDamage = Random.Range(magicDamage - (int) (magicDamage * 0.1f),
                                magicDamage + (int) (magicDamage * 0.1f));
                            currentEnemyStats.TakeDamage(DamageType.Magic, actualDamage);
                        }

                        wieldingUser.UseAP(requiredAP);
                        hitCooldownTimer += hitCooldown;

                        if (currentEnemyStats.isAlive)
                        {
                            collision.gameObject.layer = LayerMask.NameToLayer("EnemyHit");
                            foreach (Transform child in collision.gameObject.transform)
                            {
                                child.gameObject.layer = LayerMask.NameToLayer("EnemyHit");
                            }

                            enemyHit = true;
                        }

                        currentEnemyStats.EnemyDied -= OnEnemyDead; // will need to change if enemy dies outside of playerturn
                    }
                }
            }
        }

        private void OnEnemyDead()
        {
            hitCooldown = 0;
            wieldingUser.ObtainXP(currentEnemyStats.earnedXP);
        }

        private void OnCollisionExit(Collision collision)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("EnemyHit"))
            {
                inEnemyCollider = false;
            }
        }

        public void UpdateWielder()
        {
            if (grabbable.PrimaryGrabber == null)
            {
                wieldingUser = null;
                Debug.Log("Weapon dropped! wieldingUser == null");
            }
            else
            {
                wieldingUser = grabbable.PrimaryGrabber.transform.root.GetComponentInChildren<PlayerStats>();
                Debug.Log($"Weapon grabbed! wieldingUser: {wieldingUser.userName}");
            }
        }
    }
}