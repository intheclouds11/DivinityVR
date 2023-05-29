using System.Collections.Generic;
using HurricaneVR.Framework.Core.Utils;
using UnityEngine;

namespace intheclouds
{
    public class SurfaceEffect : MonoBehaviour
    {
        public StatusEffect statusEffect;
        public ScalingType ScalingType;
        public int damage;
        public int cooldown = 5;
        public int cooldownTimer;
        public AudioClip activatedAudioClip;
        public AudioClip removedAudioClip;
        public GameObject removeVFX;
        public BaseStats caster;
        private List<BaseStats> _combatantsInTrigger = new List<BaseStats>();

        private void OnTriggerEnter(Collider other)
        {
            ActivateSurfaceEffect(other);
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player") || other.CompareTag("Enemy"))
            {
                var combatant = other.gameObject.GetComponentInParent<BaseStats>();
                _combatantsInTrigger.Remove(combatant);
            }
        }

        private void ActivateSurfaceEffect(Collider other)
        {
            if (other.CompareTag("Player") || other.CompareTag("Enemy"))
            {
                var combatant = other.gameObject.GetComponentInParent<BaseStats>();
                foreach (var combatantInTrigger in _combatantsInTrigger)
                {
                    if (combatant == combatantInTrigger)
                    {
                        return;
                    }
                }
                _combatantsInTrigger.Add(combatant);
                if (ScalingType == ScalingType.Pyrokinetic)
                {
                    FireSurfaceDamage(combatant);
                }
                else if (ScalingType == ScalingType.Hydrosophist)
                {
                    Helpers.AddWetStatus(combatant, statusEffect);
                    PlayActivationSound();
                }
            }
        }

        private void FireSurfaceDamage(BaseStats combatant)
        {
            if (combatant)
            {
                damage *= caster.level * (1 + caster.Pyrokinetic);
                combatant.TakeDamage(caster, Helpers.CalculateDamageRange(damage, caster), DamageType.Magic, ScalingType, statusEffect);
            }

            PlayActivationSound();
        }

        private void PlayActivationSound()
        {
            SFXPlayer.Instance.PlaySFX(activatedAudioClip, transform.position, 1, 1);
        }
    }
}