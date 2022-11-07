using HurricaneVR.Framework.Core.Utils;
using UnityEngine;

namespace intheclouds
{
    public class SurfaceEffect : MonoBehaviour
    {
        public StatusEffect statusEffect;
        public ElementalType elementalType;
        public int damage;
        public int cooldown = 5;
        public int cooldownTimer;
        public AudioClip activatedAudioClip;
        public AudioClip removedAudioClip;
        public GameObject removeVFX;
        public BaseStats caster;

        private void OnTriggerEnter(Collider other)
        {
            ActivateSurfaceEffect(other);
        }

        private void ActivateSurfaceEffect(Collider other)
        {
            if (elementalType == ElementalType.Fire)
            {
                if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
                {
                    FireSurfaceDamagePlayer(other);
                    PlayActivationSound();
                }
                else if (other.CompareTag("Enemy"))
                {
                    FireSurfaceDamageEnemy(other);
                    PlayActivationSound();
                }
            }
            else if (elementalType == ElementalType.Water)
            {
                if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
                {
                    Helpers.MakePlayerWet(other, statusEffect);
                    PlayActivationSound();
                }
                else if (other.CompareTag("Enemy"))
                {
                    Helpers.MakeEnemyWet(other, statusEffect);
                    PlayActivationSound();
                }
            }
        }
        
        private void FireSurfaceDamageEnemy(Collider other)
        {
            if (other.gameObject.TryGetComponent(out BaseStats combatantDamaged))
            {
                damage *= caster.level * (1 + caster.Pyrokinetic);
                combatantDamaged.TakeDamage(caster, Helpers.CalculateDamageRange(damage, caster), DamageType.Magic, elementalType, statusEffect);
            }

            PlayActivationSound();
        }

        private void FireSurfaceDamagePlayer(Collider other)
        {
            if (other.transform.parent.gameObject.TryGetComponent(out BaseStats combatantDamaged))
            {
                damage *= caster.level * (1 + caster.Pyrokinetic);
                combatantDamaged.TakeDamage(caster, Helpers.CalculateDamageRange(damage, caster), DamageType.Magic, elementalType, statusEffect);
            }

            PlayActivationSound();
        }


        private void PlayActivationSound()
        {
            SFXPlayer.Instance.PlaySFX(activatedAudioClip, transform.position, 1, 1);
        }
    }
}