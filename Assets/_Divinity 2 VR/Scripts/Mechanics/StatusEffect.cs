using System;
using System.Collections;
using System.Collections.Generic;
using HurricaneVR.Framework.Core.Utils;
using TMPro;
using UnityEngine;

namespace intheclouds
{
    public class StatusEffect : MonoBehaviour
    {
        public StatusEffectType type;
        public int effectAmount;
        public int cooldown;
        public int cooldownTimer;
        public StatusEffectApplication effectApplication;
        public AudioClip activatedClip;

        public void SetEffectVars(StatusEffect effect)
        {
            type = effect.type;
            effectAmount = effect.effectAmount;
            cooldown = effect.cooldown;
            cooldownTimer = effect.cooldown;
            effectApplication = effect.effectApplication;
        }

        public void ActivateStatusEffect()
        {
            if (type == StatusEffectType.Burning)
            {
                if (TryGetComponent(out PlayerStats player))
                {
                    int damage = (int) (4 * (1 + player.level * 0.5f));
                    player.TakeDamage(null, damage, DamageType.Magic, ElementalType.Fire, null);
                    SFXPlayer.Instance.PlaySFX(activatedClip, player.LocalUserObjects.HVRPlayerController.gameObject.transform.position, 1, 0.5f);
                }
                else if (TryGetComponent(out EnemyStats enemy))
                {
                    int damage = (int) (4 * (1 + enemy.level * 0.5f));
                    enemy.TakeDamage(null, damage, DamageType.Magic, ElementalType.Fire, null);
                    SFXPlayer.Instance.PlaySFX(activatedClip, enemy.gameObject.transform.position, 1, 0.5f);
                }
            }
            else if (type == StatusEffectType.Wet)
            {
                if (TryGetComponent(out PlayerStats player))
                {
                    SFXPlayer.Instance.PlaySFX(activatedClip, player.LocalUserObjects.HVRPlayerController.gameObject.transform.position, 1, 0.5f);
                }
                else if (TryGetComponent(out EnemyStats enemy))
                {
                    SFXPlayer.Instance.PlaySFX(activatedClip, enemy.gameObject.transform.position, 1, 0.5f);
                }
            }
        }

        public enum StatusEffectType
        {
            Burning, // cured by water, First Aid, Armour of Frost, or Fortify
            Bleeding, // cured by First Aid, Restoration
            Poison, // cured by Restoration, First Aid, or Fortify
            Blinded, // cured by First Aid
            Wet, // removed by burning, fire, chilled, frozen, shocked, or stunned
            Chilled, // removed by Burning
            Frozen, // cured by Burning
            Crippled, // cured by First Aid, Haste
            KnockedDown, // cured by First Aid
            Shocked, // removed by wet
            Stunned, // cured by Armour of Frost
            Silenced, // cured by First Aid
            Slowed, // cured by Haste
            Regenerating,
            MagicShell,
            FavorableWind,
            None
        }

        public enum StatusEffectApplication
        {
            Damage,
            Healing,
            RestoreMagicArmor,
            IncreaseMagicArmor,
            RestorePhysicalArmor,
            IncreasePhysicalArmor,
            Wet,
            Slow,
            None,
        }
    }
}