using System;
using HurricaneVR.Framework.Core.Utils;
using UnityEngine;
using UnityEngine.Serialization;

namespace intheclouds
{
    public class StatusEffect : MonoBehaviour
    {
        public bool ProcessOnEnabled;
        public int ChanceToApply = 100;
        public StatusEffectType type;
        public int effectAmount;
        public int cooldown;
        public int cooldownTimer;
        public AudioClip activatedClip;
        public GameObject activeVFX;
        public GameObject activatedVFX;
        public BaseStats CombatantWhoApplied;
        private BaseStats combatant;
        private Transform originalParent;
        private GameObject spawnedActiveVFX;
        private GameObject spawnedActivatedVFX;

        private void OnDestroy()
        {
            if (type == StatusEffectType.Stunned)
            {
                Debug.Log("Stun removed!");
                combatant.Stun(false);
            }

            if (spawnedActiveVFX)
            {
                var particles = spawnedActiveVFX.GetComponent<ParticleSystem>();
                Destroy(spawnedActiveVFX, particles ? particles.main.duration - particles.time : 2f);
            }

            if (spawnedActivatedVFX)
            {
                var particles = spawnedActivatedVFX.GetComponent<ParticleSystem>();
                Destroy(spawnedActivatedVFX, particles ? particles.main.duration - particles.time : 2f);
            }
        }

        public void StatusEffectConstructor(StatusEffect effect, bool preExisting = false)
        {
            if (!combatant)
            {
                combatant = GetComponentInParent<BaseStats>();
            }

            type = effect.type;
            effectAmount = effect.effectAmount;
            cooldown = effect.cooldown;
            cooldownTimer = effect.cooldown;
            activatedClip = effect.activatedClip;
            ProcessOnEnabled = effect.ProcessOnEnabled;
            activeVFX = effect.activeVFX;
            activatedVFX = effect.activatedVFX;

            if (ProcessOnEnabled)
            {
                ActivateStatusEffect(preExisting);
            }
        }

        public void ActivateStatusEffect(bool preExisting = false)
        {
            int damage = (int) (effectAmount * (1 + combatant.level * 0.5f));
            
            if (type == StatusEffectType.Burning)
            {
                combatant.TakeDamage(null, damage, DamageType.Magic, ScalingType.Pyrokinetic, null);
            }
            else if (type == StatusEffectType.Regenerating)
            {
                combatant.Heal(effectAmount);
            }
            else if (type == StatusEffectType.Stunned)
            {
                combatant.Stun(true);
            }
            else if (type == StatusEffectType.Bleeding)
            {
                combatant.TakeDamage(null, damage, DamageType.Magic, ScalingType.None, null);
            }
            else
            {
                Debug.LogWarning("No status effect type assigned!");
            }

            if (activatedClip)
            {
                SFXPlayer.Instance.PlaySFX(activatedClip, combatant.attachToCombatantTransform.position, 1, 1);
            }

            if (!preExisting)
            {
                if (activeVFX && !spawnedActiveVFX)
                {
                    spawnedActiveVFX = Instantiate(activeVFX, combatant.attachToCombatantTransform.position, Quaternion.identity, combatant.attachToCombatantTransform);
                }

                if (activatedVFX)
                {
                    if (!spawnedActivatedVFX)
                    {
                        spawnedActivatedVFX = Instantiate(activatedVFX, combatant.attachToCombatantTransform.position, Quaternion.identity,
                            combatant.attachToCombatantTransform);
                    }
                    else
                    {
                        spawnedActivatedVFX.SetActive(false);
                        spawnedActivatedVFX.SetActive(true);
                    }
                }
            }
        }
    }
}