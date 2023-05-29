using System;
using HurricaneVR.Framework.Components;
using HurricaneVR.Framework.Core;
using HurricaneVR.Framework.Core.Grabbers;
using HurricaneVR.Framework.Core.Utils;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

namespace intheclouds
{
    public class ImpactHandler : MonoBehaviour, IHoverableItem
    {
        #region Variables

        [Header("Damage Handling")]
        public int requiredAP = 2;
        [Tooltip("Enable for weapons that should disable collision on successful hit so player can follow through with attack")]
        public bool disableCollisionsOnHitEnemy;
        public bool canBackstab;
        public int baseDamage = 1;
        public float criticalDamageMultiplier = 1.8f;
        public float damageThreshold = 7;
        public float hitCooldown = 0.25f;
        public DamageType damageType = DamageType.Physical;
        public ScalingType scalingType = ScalingType.None;
        public StatusEffect statusEffect;

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
        [ShowIf(nameof(showSwipe))]
        public float MaxVolumeSwipe = 1f;
        [ShowIf(nameof(showSwipe))]
        public float VolumeModifierSwipe = 2f;

        public event Action AppliedDamage;

        private AudioSource _impactAudioSource;
        private AudioSource _swipeAudioSource;
        private HVRCollisionEvents _collisionEvents;
        private bool _justHit;
        private PlayerStats _wieldingUser;
        private Rigidbody _rb;
        private HVRGrabbable _grabbable;
        private bool _isPlayingSFX;
        private float _pitch;
        private float _volume;
        private Vector3 _lastAngularVelocity;
        private Collider[] _enemyColliders;

        private bool showSwipe => SwipeClip;

        #endregion


        private void Start()
        {
            _collisionEvents = GetComponent<HVRCollisionEvents>();
            _rb = GetComponent<Rigidbody>();
            _grabbable = GetComponent<HVRGrabbable>();
            GetComponent<HVRGrabbable>().Grabbed.AddListener(AssignWielder);
        }

        private void AssignWielder(HVRGrabberBase grabber, HVRGrabbable grabbable)
        {
            _wieldingUser = grabber.GetComponentInParent<PlayerStats>();
        }

        private void Update()
        {
            HandleSwipeSFX();
        }

        private void HandleSwipeSFX()
        {
            if (_grabbable.IsSocketed || !SwipeClip)
            {
                return;
            }

            if (_grabbable.IsHandGrabbed && _wieldingUser)
            {
                var acceleration = Mathf.Abs(_rb.angularVelocity.magnitude - _lastAngularVelocity.magnitude) * Time.fixedDeltaTime;
                _lastAngularVelocity = _rb.angularVelocity;

                // var wielderVelocity = wieldingUser.LocalUserObjects.HVRPlayerController.CharacterController.velocity.magnitude;
                // var velocityRelativeToWielder = Mathf.Abs(wielderVelocity - rb.velocity.magnitude);

                if ((!_swipeAudioSource || !_swipeAudioSource.isPlaying) && acceleration > SwipeThreshold)
                {
                    // todo use HVRUtilities.Remap() to make volume scale better
                    _swipeAudioSource = PlayVelocityBasedSFX(acceleration, SwipeClip, MinPitchSwipe, MaxPitchSwipe, MaxVolumeSwipe, 10, VolumeModifierSwipe);
                }
                else if (_swipeAudioSource && acceleration < SwipeCooldownThreshold)
                {
                    StartCoroutine(HVRUtilities.FadeOut(_swipeAudioSource, 0.2f));
                }
            }
        }

        // Handles impact sfx and damage caused by prop
        private void OnCollisionEnter(Collision collision)
        {
            var relativeVelocity = collision.relativeVelocity.magnitude;

            // Prevent impact sfx playing same time as destroy sfx
            if (relativeVelocity > ImpactThreshold && (!_collisionEvents || relativeVelocity <= _collisionEvents.VelocityThreshold))
            {
                HandleImpactSFX(relativeVelocity);
            }

            if (baseDamage == 0 || _justHit || !_wieldingUser || !_wieldingUser.CanPerformActions(requiredAP))
            {
                return;
            }

            var objectDamageHandler = collision.collider.GetComponent<HVRDamageHandlerBase>();
            if (objectDamageHandler && relativeVelocity >= damageThreshold)
            {
                DamageDestructible(objectDamageHandler, relativeVelocity);
            }

            if (!disableCollisionsOnHitEnemy && collision.gameObject.CompareTag("EnemyBody") || collision.gameObject.CompareTag("EnemyHead"))
            {
                if (relativeVelocity > damageThreshold)
                {
                    DamageEnemy(collision.collider, relativeVelocity, false);
                }
            }
        }

        // Handles damage caused by weapon. Disables collisions after valid hit
        private void OnTriggerEnter(Collider other)
        {
            if (!disableCollisionsOnHitEnemy || baseDamage == 0 || _justHit || !_wieldingUser || !_wieldingUser.CanPerformActions(requiredAP))
            {
                return;
            }

            if (other.CompareTag("EnemyBody") || other.CompareTag("EnemyHead"))
            {
                if (_rb.velocity.magnitude > damageThreshold || _rb.angularVelocity.magnitude > damageThreshold * 3)
                {
                    DamageEnemy(other, _rb.velocity.magnitude, true);
                }
            }
        }

        private void DamageDestructible(HVRDamageHandlerBase objectDamageHandler, float relativeVelocity)
        {
            var scaledDamage = (int) Math.Ceiling(baseDamage * (_wieldingUser.Strength * 0.105f));
            objectDamageHandler.TakeDamage(scaledDamage);
            if (_wieldingUser.InCombat)
            {
                _wieldingUser.UseAP(requiredAP);
            }

            PlayVelocityBasedSFX(relativeVelocity, GenericHitClip, MinPitch, MaxPitch, MaxVolume);
            _justHit = true;
            AppliedDamage?.Invoke();
            Invoke(nameof(ResetCollision), hitCooldown);
        }


        private void DamageEnemy(Collider hitCollider, float relativeVelocity, bool ignoreCollisionsPostHit)
        {
            var currentEnemyStats = hitCollider.gameObject.GetComponentInParent<EnemyStats>();
            if (!currentEnemyStats.isAlive) return;

            // 0.105 comes from dividing base strength (10) by 10 and multiplying 1.05 (5%+). every strength point is 5% damage boost
            var scaledDamage = (int) Math.Ceiling(baseDamage * (_wieldingUser.Strength * 0.105f));
            if (canBackstab && _wieldingUser.CanBackstab && _wieldingUser.BackstabTargets.Contains(currentEnemyStats))
            {
                scaledDamage *= 2;
            }

            currentEnemyStats.TakeDamage(_wieldingUser, Helpers.CalculateDamageRange(scaledDamage, _wieldingUser, criticalDamageMultiplier),
                damageType, scalingType, statusEffect);

            if (_wieldingUser.InCombat)
            {
                _wieldingUser.UseAP(requiredAP);
            }

            PlayVelocityBasedSFX(relativeVelocity, HitEnemyClip, MinPitch, MaxPitch, MaxVolume);
            _justHit = true;
            if (ignoreCollisionsPostHit)
            {
                _enemyColliders = currentEnemyStats.GetComponentsInChildren<Collider>();
                IgnoreCollision(_enemyColliders);
            }

            AppliedDamage?.Invoke();
            Invoke(nameof(ResetCollision), hitCooldown);
        }

        public void IgnoreCollision(Collider[] other, bool ignore = true)
        {
            if (other == null) return;
            foreach (var otherCollider in other)
            {
                foreach (var ourCollider in _grabbable.Colliders)
                {
                    Physics.IgnoreCollision(otherCollider, ourCollider, ignore);
                }
            }
        }

        private void HandleImpactSFX(float relativeVelocity)
        {
            if (!_impactAudioSource || !_impactAudioSource.isPlaying)
            {
                _impactAudioSource = PlayVelocityBasedSFX(relativeVelocity, GenericHitClip, MinPitch, MaxPitch, MaxVolume);
            }
        }

        private void ResetCollision()
        {
            _justHit = false;
            IgnoreCollision(_enemyColliders, false);
            _enemyColliders = null;
        }

        private AudioSource PlayVelocityBasedSFX(float relativeVelocity, AudioClip clip, float minP, float maxP, float maxVol, float pitchModifier = 0.3f,
            float volumeModifier = 0.25f)
        {
            if (!clip)
            {
                clip = GenericHitClip;
            }

            if (clip)
            {
                _pitch = Mathf.Clamp(relativeVelocity * pitchModifier, minP, maxP);
                _volume = Mathf.Clamp(relativeVelocity * volumeModifier, 0, maxVol);
                return SFXPlayer.Instance.PlaySFX(clip, transform.position, _pitch, _volume, 20);
            }

            return null;
        }

        public string GetHoverInfo()
        {
            if (baseDamage > 0)
            {
                return $"{name}, Damage: {baseDamage}";
            }

            return $"{name}";
        }
    }
}