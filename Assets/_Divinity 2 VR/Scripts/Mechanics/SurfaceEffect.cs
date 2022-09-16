using System;
using System.Collections;
using System.Collections.Generic;
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
        public AudioClip damageClip;
        public BaseStats caster;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                Debug.Log($"{other.gameObject.name} stepped on burning surface!");
                if (other.transform.parent.gameObject.TryGetComponent(out BaseStats combatantDamaged))
                {
                    combatantDamaged.TakeDamage(caster, Helpers.CalculateDamageRange(damage, caster), DamageType.Magic, elementalType, statusEffect);
                }

                SFXPlayer.Instance.PlaySFXAttach(damageClip, transform, 1, 1);
            }
            else if (other.CompareTag("Enemy"))
            {
                if (other.gameObject.TryGetComponent(out BaseStats combatantDamaged))
                {
                    combatantDamaged.TakeDamage(caster, Helpers.CalculateDamageRange(damage, caster), DamageType.Magic, elementalType, statusEffect);
                }

                SFXPlayer.Instance.PlaySFXAttach(damageClip, transform, 1, 1);
            }
        }
    }
}