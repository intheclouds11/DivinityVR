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
        public float lowSpeedHitGeneric = 5f;
        public float medSpeedHitGeneric = 10f;
        public float fastSpeedHitGeneric = 15f;
        public int requiredAP = 2;
        public int physicalDamage = 10;
        public int magicDamage = 0;
        private PlayerStats wieldingUser;
        private Rigidbody rb;
        private HVRGrabbable grabbable;
        public float hitCooldownTimer;
        private bool inEnemyCollider;
        private Collision currentEnemyCollision;
        private GameObject enemyRoot;
        private EnemyStats currentEnemyStats;

        public AudioSource hitSFXAudioSource;

        public AudioClip genericHitClip;
        public AudioClip enemyHitClip;

        private void Start()
        {
            grabbable = GetComponent<HVRGrabbable>();
            rb = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            if (wieldingUser == null) return;

            if (!inEnemyCollider && hitCooldownTimer > 0)
            {
                hitCooldownTimer -= Time.deltaTime;
            }

            else if (enemyRoot != null && !inEnemyCollider && hitCooldownTimer <= 0)
            {
                SetLayerRecursively(enemyRoot, LayerMask.NameToLayer("Enemy"));
                enemyRoot = null;
            }
        }

        void SetLayerRecursively(GameObject obj, int newLayer)
        {
            if (null == obj)
            {
                return;
            }

            obj.layer = newLayer;

            foreach (Transform child in obj.transform)
            {
                if (null == child)
                {
                    continue;
                }

                SetLayerRecursively(child.gameObject, newLayer);
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                if (hitCooldownTimer > 0 || wieldingUser == null) return;

                inEnemyCollider = true;

                if (wieldingUser.currentAP > requiredAP)
                {
                    if (collision.relativeVelocity.magnitude > lowSpeedHitEnemy)
                    {
                        if (!wieldingUser.turn && !wieldingUser.explorationMode) return;
                        enemyRoot = collision.gameObject.transform.root.GetComponentInChildren<EnemyStats>().gameObject;
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

                        if (!wieldingUser.explorationMode)
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

                        hitCooldownTimer += hitCooldown;

                        if (currentEnemyStats.isAlive)
                        {
                            SetLayerRecursively(enemyRoot, LayerMask.NameToLayer("EnemyHit"));
                        }

                        currentEnemyStats.EnemyDied -= OnEnemyDead; // will need to change if enemy dies outside of playerturn
                    }
                }
            }
            else
            {
                if (collision.relativeVelocity.magnitude > lowSpeedHitGeneric)
                {
                    hitSFXAudioSource.pitch = 1 - Mathf.Clamp(collision.relativeVelocity.magnitude / lowSpeedHitGeneric, 0f, 0.2f); // todo: not getting varied pitch
                    hitSFXAudioSource.PlayOneShot(genericHitClip);
                }
            }
        }

        private void OnEnemyDead()
        {
            hitCooldown = 0;
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
                Debug.Log($"Weapon grabbed! wieldingUser: {wieldingUser.Name}");
            }
        }
    }
}