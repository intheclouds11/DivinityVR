using System;
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
        private BaseStats combatant;
        private TextMeshProUGUI textUI;
        private string damageString;
        private string healingString;

        private void Start()
        {
            combatant = GetComponent<BaseStats>();
            textUI = combatant.statusEffectsText;
        }

        void Update()
        {
            if (!combatant.InCombat)
            {
                CooldownExploration();
            }
        }

        public void CooldownExploration()
        {
            if (cooldownTimerNoCombat < 2)
            {
                cooldownTimerNoCombat += Time.deltaTime;
            }
            else if (cooldownTimerNoCombat >= 2)
            {
                Cooldown();
                cooldownTimerNoCombat = 0;
            }
        }

        public void Cooldown()
        {
            if (statusEffectList.Count > 0)
            {
                TryProcessStatusEffects();

                for (int i = 0; i < statusEffectList.Count; i++)
                {
                    if (statusEffectList[i].cooldownTimer > 0)
                    {
                        statusEffectList[i].cooldownTimer -= 1;
                        if (statusEffectList[i].effectApplication == StatusEffect.StatusEffectApplication.Damage)
                        {
                            RemoveFromTextUI(statusEffectList[i].effectApplication);
                            AddToTextUI(statusEffectList[i]);
                        }
                    }

                    if (statusEffectList[i].cooldownTimer == 0)
                    {
                        RemoveStatusEffect(i);
                        i--;
                    }
                }
            }
        }

        public void RemoveStatusEffect(int i)
        {
            Debug.Log($"Removing {statusEffectList[i].name} status effect");
            RemoveFromTextUI(statusEffectList[i].effectApplication);
            Destroy(statusEffectList[i]);
            statusEffectList.RemoveAt(i);
        }

        public void RemoveStatusEffect(StatusEffect effect)
        {
            Debug.Log($"Removing {effect.name} status effect");
            Destroy(effect);
            RemoveFromTextUI(effect.effectApplication);
            statusEffectList.Remove(effect);
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
                RemoveFromTextUI(statusEffect.effectApplication);
                AddToTextUI(statusEffect);
                Debug.Log("status effect reapplied");
            }
            else
            {
                var appliedEffect = gameObject.AddComponent<StatusEffect>();
                appliedEffect.SetEffectVars(statusEffect);
                statusEffectList.Add(appliedEffect);
                AddToTextUI(statusEffect);
                Debug.Log("status effect applied (first time)");
            }
        }

        private void AddToTextUI(StatusEffect statusEffect)
        {
            if (textUI.text != "")
            {
                textUI.text += $"\n";
            }

            if (statusEffect.effectApplication == StatusEffect.StatusEffectApplication.Damage)
            {
                damageString = $"{statusEffect.type.ToString()} damages {statusEffect.effectAmount} vitality for {statusEffect.cooldownTimer} more turn(s)";
                textUI.text += damageString;
            }
            else if (statusEffect.effectApplication == StatusEffect.StatusEffectApplication.Healing)
            {
                healingString = $"{statusEffect.type.ToString()} heals {statusEffect.effectAmount} vitality for {statusEffect.cooldownTimer} more turn(s)";
                textUI.text += healingString;
            }
            else if (statusEffect.effectApplication == StatusEffect.StatusEffectApplication.RestoreMagicArmor)
            {
                textUI.text += $"{statusEffect.type.ToString()} restores {statusEffect.effectAmount} magic armor for {statusEffect.cooldownTimer} more turn(s)";
            }
            else if (statusEffect.effectApplication == StatusEffect.StatusEffectApplication.RestorePhysicalArmor)
            {
                textUI.text += $"{statusEffect.type.ToString()} restores {statusEffect.effectAmount} physical armor for {statusEffect.cooldownTimer} more turn(s)";
            }
            else if (statusEffect.effectApplication == StatusEffect.StatusEffectApplication.IncreaseMagicArmor)
            {
                textUI.text += $"{statusEffect.type.ToString()} boosts magic armor by {statusEffect.effectAmount} for {statusEffect.cooldownTimer} more turn(s)";
            }
            else if (statusEffect.effectApplication == StatusEffect.StatusEffectApplication.IncreasePhysicalArmor)
            {
                textUI.text += $"{statusEffect.type.ToString()} boosts physical armor by {statusEffect.effectAmount} for {statusEffect.cooldownTimer} more turn(s)";
            }
        }

        private void RemoveFromTextUI(StatusEffect.StatusEffectApplication effectApplication)
        {
            if (effectApplication == StatusEffect.StatusEffectApplication.Damage)
            {
                textUI.text = textUI.text.Replace(damageString, string.Empty);
            }
        }
    }
}