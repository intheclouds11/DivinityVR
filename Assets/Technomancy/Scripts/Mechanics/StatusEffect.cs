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
        private BaseStats combatant;
        private BaseStats combatantWhoApplied;

        private void OnDestroy()
        {
            if (type == StatusEffectType.Stunned)
            {
                Debug.Log("Stun removed!");
                combatant.Stun(false);
            }
        }

        public void StatusEffectConstructor(StatusEffect effect)
        {
            type = effect.type;
            effectAmount = effect.effectAmount;
            cooldown = effect.cooldown;
            cooldownTimer = effect.cooldown;
            activatedClip = effect.activatedClip;
            ProcessOnEnabled = effect.ProcessOnEnabled;
            if (ProcessOnEnabled)
            {
                ActivateStatusEffect();
            }
        }

        public void ActivateStatusEffect()
        {
            if (!combatant)
            {
                combatant = GetComponentInParent<BaseStats>();
            }
            
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
                SFXPlayer.Instance.PlaySFX(activatedClip, combatant.transform.GetChild(0).position, 1, 10);
            }
        }
    }
}