using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace intheclouds
{
    public class StatusEffectsContainer : MonoBehaviour
    {
        public List<StatusEffect> statusEffectList;
        private float cooldownTimerNoCombat;

        void Update()
        {
        }

        public void TryProcessStatusEffects()
        {
            foreach (var statusEffect in statusEffectList)
            {
                statusEffect.ActivateEffect();
            }
        }

        public void TryAddStatusEffect(StatusEffect statusEffect)
        {
            if (!statusEffect || statusEffect.type == StatusEffect.StatusEffectType.None)
            {
                return;
            }

            if (TryGetComponent(out StatusEffect preExistingEffect))
            {
                preExistingEffect.SetEffectVars(statusEffect);
                Debug.Log("status effect reapplied");
            }
            else
            {
                var appliedEffect = gameObject.AddComponent<StatusEffect>();
                appliedEffect.SetEffectVars(statusEffect);
                statusEffectList.Add(appliedEffect);
                Debug.Log("status effect applied (first time)");
            }

            UpdateTextUI(statusEffect);
        }

        private void UpdateTextUI(StatusEffect statusEffect)
        {
            var textMeshProText = GetComponent<PlayerStats>().statusEffectsText;
            if (textMeshProText.text != "")
            {
                textMeshProText.text += $"\n";
            }

            if (statusEffect.effectApplication == StatusEffect.StatusEffectApplication.Damage)
            {
                textMeshProText.text += $"{statusEffect.name} damages {statusEffect.effectAmount} vitality for {statusEffect.cooldownTimer} more turn(s)";
            }
            else if (statusEffect.effectApplication == StatusEffect.StatusEffectApplication.Healing)
            {
                textMeshProText.text += $"{statusEffect.name} heals {statusEffect.effectAmount} vitality for {statusEffect.cooldownTimer} more turn(s)";
            }
            else if (statusEffect.effectApplication == StatusEffect.StatusEffectApplication.RestoreMagicArmor)
            {
                textMeshProText.text += $"{statusEffect.name} restores {statusEffect.effectAmount} magic armor for {statusEffect.cooldownTimer} more turn(s)";
            }
            else if (statusEffect.effectApplication == StatusEffect.StatusEffectApplication.RestorePhysicalArmor)
            {
                textMeshProText.text += $"{statusEffect.name} restores {statusEffect.effectAmount} physical armor for {statusEffect.cooldownTimer} more turn(s)";
            }
            else if (statusEffect.effectApplication == StatusEffect.StatusEffectApplication.IncreaseMagicArmor)
            {
                textMeshProText.text += $"{statusEffect.name} boosts magic armor by {statusEffect.effectAmount} for {statusEffect.cooldownTimer} more turn(s)";
            }
            else if (statusEffect.effectApplication == StatusEffect.StatusEffectApplication.IncreasePhysicalArmor)
            {
                textMeshProText.text += $"{statusEffect.name} boosts physical armor by {statusEffect.effectAmount} for {statusEffect.cooldownTimer} more turn(s)";
            }
        }

        public void CooldownExploration()
        {
        }

        public void Cooldown()
        {
            if (statusEffectList.Count > 0)
            {
                for (int i = 0; i < statusEffectList.Count; i++)
                {
                    if (statusEffectList[i].cooldownTimer > 0)
                    {
                        statusEffectList[i].cooldownTimer -= 1;
                    }

                    if (statusEffectList[i].cooldownTimer == 0)
                    {
                        statusEffectList.RemoveAt(i--);
                    }
                }
            }
        }
    }
}