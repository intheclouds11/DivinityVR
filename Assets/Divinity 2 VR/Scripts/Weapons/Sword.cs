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
        public float lowSpeedHitEnemy = 5f;
        public float medSpeedHitEnemy = 10f;
        public float fastSpeedHitEnemy = 15f;
        public int requiredAP = 2;
        public int physicalDamage = 10;
        public int magicDamage = 0;
        private PlayerStats wieldingUser;
        private HVRGrabbable grabbable;
        private bool inEnemyCollider;
        private Collision currentEnemyCollision;
        private EnemyStats currentEnemyStats;
        private bool canDamage = true;

        public AudioSource hitSFXAudioSource;
        public AudioClip enemyHitClip;

        private void Start()
        {
            grabbable = GetComponent<HVRGrabbable>();
        }

        private void Update()
        {
            if (wieldingUser == null) return;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                if (!canDamage || wieldingUser == null || !wieldingUser.Turn && !wieldingUser.explorationMode) return;

                if (wieldingUser.CurrentAP >= requiredAP && collision.relativeVelocity.magnitude > lowSpeedHitEnemy)
                {
                    canDamage = false;
                    currentEnemyCollision = collision;
                    hitSFXAudioSource.pitch = 1 - Mathf.Clamp(collision.relativeVelocity.magnitude * 0.1f, 0f, 0.2f); // todo: not getting varied pitch
                    hitSFXAudioSource.PlayOneShot(enemyHitClip);

                    currentEnemyStats = collision.gameObject.GetComponentInParent<EnemyStats>();
                    currentEnemyStats.EnemyDied += OnEnemyDead;

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

                    if (currentEnemyStats.isAlive || !wieldingUser.explorationMode)
                    {
                        wieldingUser.UseAP(requiredAP);
                    }
                    else
                    {
                        if (!ITCPlayerInputs.Instance.debugInteractions)
                        {
                            wieldingUser.explorationMode = false;
                        }
                    }

                    if (currentEnemyStats.isAlive)
                    {
                        currentEnemyCollision.collider.enabled = false;
                        Invoke(nameof(ResetCollision), hitCooldown);
                    }

                    currentEnemyStats.EnemyDied -= OnEnemyDead; // will need to change if enemy dies outside of playerturn
                }
            }
        }

        public void OnEnemyDead()
        {
        }
        

        private void ResetCollision()
        {
            currentEnemyCollision.collider.enabled = true;
            canDamage = true;
        }

        public void UpdateWielder()
        {
            wieldingUser = grabbable.PrimaryGrabber.transform.root.GetComponent<LocalUserObjects>().PlayerStats;
        }
    }
}