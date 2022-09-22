using System;
using HurricaneVR.Framework.Core.Utils;
using UnityEngine;
using UnityEngine.Serialization;

namespace intheclouds
{
    public class SurfaceEffect : MonoBehaviour
    {
        public StatusEffect statusEffect;
        public ElementalType elementalType;
        public int damage;
        public int cooldown = 5;
        public int cooldownTimer;
        public AudioClip damageAudioClip;
        public AudioClip removeAudioClip;
        public GameObject removeVFX;
        public BaseStats caster;
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                Debug.Log($"{other.gameObject.name} stepped on burning surface!");
                if (other.transform.parent.gameObject.TryGetComponent(out BaseStats combatantDamaged))
                {
                    damage *= caster.level * (1 + caster.Pyrokinetic);
                    combatantDamaged.TakeDamage(caster, Helpers.CalculateDamageRange(damage, caster), DamageType.Magic, elementalType, statusEffect);
                }

                SFXPlayer.Instance.PlaySFXAttach(damageAudioClip, transform, 1, 1);
            }
            else if (other.CompareTag("Enemy"))
            {
                if (other.gameObject.TryGetComponent(out BaseStats combatantDamaged))
                {
                    damage *= caster.level * (1 + caster.Pyrokinetic);
                    combatantDamaged.TakeDamage(caster, Helpers.CalculateDamageRange(damage, caster), DamageType.Magic, elementalType, statusEffect);
                }

                SFXPlayer.Instance.PlaySFXAttach(damageAudioClip, transform, 1, 1);
            }
        }

        private void OnParticleCollision(GameObject other)
        {
            Debug.Log(other);
            Debug.Log(other.name);
            Debug.Log(other.gameObject);
            Debug.Log(other.gameObject.name);
            SurfaceEffectsContainer.Instance.RemoveSurfaceEffect(this);
        }
    }
}