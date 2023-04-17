using System;
using HurricaneVR.Framework.Core.Utils;
using UnityEngine;
using UnityEngine.Serialization;

namespace intheclouds
{
    public class StatusEffect : MonoBehaviour
    {
        public bool ProcessOnEnabled;
        public int ChanceToApply;
        public StatusEffectType type;
        public int effectAmount;
        public int cooldown;
        public int cooldownTimer;
        public AudioClip activatedClip;
        public GameObject activeVFX;
        public BaseStats CombatantWhoApplied;
        private BaseStats combatant;
        private Transform originalParent;
        private GameObject spawnedActiveVFX;

        private void OnDestroy()
        {
            if (type == StatusEffectType.Stunned)
            {
                Debug.Log("Stun removed!");
                combatant.Stun(false);
            }
            
            if (spawnedActiveVFX)
            {
                Destroy(spawnedActiveVFX);
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
            
            if (ProcessOnEnabled)
            {
                ActivateStatusEffect(preExisting);
            }
        }

        public void ActivateStatusEffect(bool preExisting = false)
        {
            if (type == StatusEffectType.Burning)
            {
                int damage = (int) (4 * (1 + combatant.level * 0.5f));
                combatant.TakeDamage(null, damage, DamageType.Magic, ScalingType.Pyrokinetic, null);
            }
            else if (type == StatusEffectType.Regenerating)
            {
                combatant.Heal(effectAmount);
            }
            else if (type == StatusEffectType.Stunned)
            {
                Debug.Log("Stunned!");
                combatant.Stun(true);
            }
            else
            {
                Debug.LogWarning("No status effect type assigned!");
            }

            if (activatedClip)
            {
                SFXPlayer.Instance.PlaySFXRandomPitch(activatedClip, transform.parent.position, 0.9f, 1, 1);
            }

            if (activeVFX && !preExisting && !spawnedActiveVFX)
            {
                spawnedActiveVFX = Instantiate(activeVFX, transform.parent.position + Vector3.up, Quaternion.identity, transform.parent);
            }
        }
    }
}