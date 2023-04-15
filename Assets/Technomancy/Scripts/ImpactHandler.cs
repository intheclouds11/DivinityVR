using System;
using HurricaneVR.Framework.Components;
using HurricaneVR.Framework.Core;
using HurricaneVR.Framework.Core.Grabbers;
using HurricaneVR.Framework.Core.Utils;
using NaughtyAttributes;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace intheclouds
{
    public class ImpactHandler : MonoBehaviour
    {
        #region Variables

        [Header("Damage Handling")]
        public bool DisableCollisionsOnHitEnemy;
        public int RequiredAP = 1;
        public int BaseDamage = 1;
        public float CriticalDamageMultiplier = 1.8f;
        public float DamageThreshold = 5;
        public float HitCooldown = 0.25f;
        public DamageType DamageType = DamageType.Physical;
        public ScalingType ScalingType = ScalingType.None;
        public StatusEffect StatusEffect;

        [Header("SFX Handling")]
        public AudioClip HitEnemyClip;
        public AudioClip GenericHitClip;
        public float ImpactThreshold = 0.01f;
        public float MinPitch = 0.95f;
        public float MaxPitch = 1;
        public float MaxVolume = 0.5f;
        public AudioClip SwipeClip;
        [ShowIf("showSwipe")]
        public float SwipeThreshold = 3f;
        [ShowIf("showSwipe")]
        public float SwipeCooldownThreshold = 1f;
        [ShowIf("showSwipe")]
        public float MinPitchSwipe = 1f;
        [ShowIf("showSwipe")]
        public float MaxPitchSwipe = 1;
        [ShowIf("showSwipe")]
        public float MaxVolumeSwipe = 1f;
        [ShowIf("showSwipe")]
        public float VolumeModifierSwipe = 2f;

        public event Action AppliedDamage;

        private AudioSource impactAudioSource;
        private AudioSource swipeAudioSource;
        private HVRCollisionEvents collisionEvents;
        private bool justHit;
        private PlayerStats wieldingUser;
        private Rigidbody rb;
        private HVRGrabbable grabbable;
        private bool isPlayingSFX;
        private float pitch;
        private float volume;
        private Vector3 lastAngularVelocity;

        private bool showSwipe => SwipeClip;

        #endregion


        private void Start()
        {
            collisionEvents = GetComponent<HVRCollisionEvents>();
            rb = GetComponent<Rigidbody>();
            grabbable = GetComponent<HVRGrabbable>();
            GetComponent<HVRGrabbable>().Grabbed.AddListener(AssignWielder);
        }

        private void AssignWielder(HVRGrabberBase arg0, HVRGrabbable arg1)
        {
            wieldingUser = arg0.GetComponentInParent<PlayerStats>();
        }

        private void Update()
        {
            HandleSwipeSFX();
        }

        private void HandleSwipeSFX()
        {
            if (grabbable.IsSocketed || !SwipeClip)
            {
                return;
            }

            if (grabbable.IsHandGrabbed && wieldingUser)
            {
                var acceleration = Mathf.Abs(rb.angularVelocity.magnitude - lastAngularVelocity.magnitude) * Time.fixedDeltaTime;
                lastAngularVelocity = rb.angularVelocity;

                // var wielderVelocity = wieldingUser.LocalUserObjects.HVRPlayerController.CharacterController.velocity.magnitude;
                // var velocityRelativeToWielder = Mathf.Abs(wielderVelocity - rb.velocity.magnitude);

                if ((!swipeAudioSource || !swipeAudioSource.isPlaying) && acceleration > SwipeThreshold)
                {
                    // todo use HVRUtilities.Remap() to make volume scale better
                    swipeAudioSource = PlayVelocityBasedSFX(acceleration, SwipeClip, MinPitchSwipe, MaxPitchSwipe, MaxVolumeSwipe, 10, VolumeModifierSwipe);
                }
                else if (swipeAudioSource && acceleration < SwipeCooldownThreshold)
                {
                    StartCoroutine(HVRUtilities.FadeOut(swipeAudioSource, 0.2f));
                }
            }
        }

        // Handles impact sfx and damage caused by prop
        private void OnCollisionEnter(Collision collision)
        {
            var relativeVelocity = collision.relativeVelocity.magnitude;

            // Prevent impact sfx playing same time as destroy sfx
            if (relativeVelocity > ImpactThreshold && (!collisionEvents || relativeVelocity <= collisionEvents.VelocityThreshold))
            {
                HandleImpactSFX(relativeVelocity);
            }

            if (BaseDamage == 0 || justHit || !wieldingUser || !wieldingUser.CanPerformActions(RequiredAP))
            {
                return;
            }
            
            var objectDamageHandler = collision.collider.GetComponent<HVRDamageHandlerBase>();
            if (objectDamageHandler && relativeVelocity >= DamageThreshold)
            {
                DamageDestructible(objectDamageHandler, relativeVelocity);
            }

            if (!DisableCollisionsOnHitEnemy && collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                if (relativeVelocity > DamageThreshold)
                {
                    DamageEnemy(collision.collider, relativeVelocity, false);
                }
            }
        }

        // Handles damage caused by weapon. Disables collisions after valid hit
        private void OnTriggerEnter(Collider other)
        {
            if (!DisableCollisionsOnHitEnemy || BaseDamage == 0 || justHit || !wieldingUser || !wieldingUser.CanPerformActions(RequiredAP))
            {
                return;
            }

            if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                if (rb.velocity.magnitude > DamageThreshold || rb.angularVelocity.magnitude > DamageThreshold * 5)
                {
                    DamageEnemy(other, rb.velocity.magnitude, true);
                }
            }
        }

        private void DamageDestructible(HVRDamageHandlerBase objectDamageHandler, float relativeVelocity)
        {
            var scaledDamage = (int) Math.Ceiling(BaseDamage * (wieldingUser.Strength * 0.105f));
            objectDamageHandler.TakeDamage(scaledDamage);
            if (wieldingUser.InCombat)
            {
                wieldingUser.UseAP(RequiredAP);
            }

            PlayVelocityBasedSFX(relativeVelocity, GenericHitClip, MinPitch, MaxPitch, MaxVolume);
            justHit = true;
            AppliedDamage?.Invoke();
            Invoke(nameof(ResetCollision), HitCooldown);
        }


        private void DamageEnemy(Collider hitCollider, float relativeVelocity, bool disableRbCollision)
        {
            if (hitCollider.isTrigger)
            {
                return;
            }
            var currentEnemyStats = hitCollider.gameObject.GetComponentInParent<EnemyStats>();
            if (!currentEnemyStats.isAlive) return;

            var scaledDamage = (int) Math.Ceiling(BaseDamage * (wieldingUser.Strength * 0.105f));
            // 0.105 comes from dividing base strength (10) by 10 and multiplying 1.05 (5%+). every strength point is 5% damage boost
            currentEnemyStats.TakeDamage(wieldingUser, Helpers.CalculateDamageRange(scaledDamage, wieldingUser, CriticalDamageMultiplier),
                DamageType, ScalingType, StatusEffect);

            if (wieldingUser.InCombat)
            {
                wieldingUser.UseAP(RequiredAP);
            }

            PlayVelocityBasedSFX(relativeVelocity, HitEnemyClip, MinPitch, MaxPitch, MaxVolume);
            justHit = true;
            if (disableRbCollision)
            {
                rb.detectCollisions = false;
            }
            AppliedDamage?.Invoke();
            Invoke(nameof(ResetCollision), HitCooldown);
        }

        private void HandleImpactSFX(float relativeVelocity)
        {
            if (!impactAudioSource || !impactAudioSource.isPlaying)
            {
                impactAudioSource = PlayVelocityBasedSFX(relativeVelocity, GenericHitClip, MinPitch, MaxPitch, MaxVolume);
            }
        }

        private void ResetCollision()
        {
            justHit = false;
            rb.detectCollisions = true;
        }

        private AudioSource PlayVelocityBasedSFX(float relativeVelocity, AudioClip clip, float minP, float maxP, float maxVol, float pitchModifier = 0.3f, float volumeModifier = 0.25f)
        {
            if (clip)
            {
                pitch = Mathf.Clamp(relativeVelocity * pitchModifier, minP, maxP);
                volume = Mathf.Clamp(relativeVelocity * volumeModifier, 0, maxVol);
                return SFXPlayer.Instance.PlaySFX(clip, transform.position, pitch, volume, 20);
            }

            return null;
        }
    }
}