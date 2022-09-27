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
        private string burningText;
        private string regeneratingText;
        private string wetText;
        private string slowText;
        private string magicShellText;

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
                    var statusEffect = statusEffectList[i];
                    if (statusEffect.cooldownTimer > 0)
                    {
                        statusEffect.cooldownTimer -= 1;
                        RemoveFromTextUI(statusEffect);
                        AddToTextUI(statusEffect);
                    }

                    if (statusEffect.cooldownTimer == 0)
                    {
                        RemoveStatusEffect(i--);
                    }
                }
            }
        }

        public void RemoveStatusEffect(int i)
        {
            Debug.Log($"Removing {statusEffectList[i].name} status effect");
            RemoveFromTextUI(statusEffectList[i]);
            Destroy(statusEffectList[i]);
            statusEffectList.RemoveAt(i);
        }

        public void RemoveStatusEffect(StatusEffect effect)
        {
            Debug.Log($"Removing {effect.name} status effect");
            Destroy(effect);
            RemoveFromTextUI(effect);
            statusEffectList.Remove(effect);
        }

        public void TryProcessStatusEffects()
        {
            foreach (var statusEffect in statusEffectList)
            {
                statusEffect.ActivateStatusEffect();
            }
        }

        public void TryAddStatusEffect(StatusEffect statusEffect)
        {
            if (!statusEffect || statusEffect.type == StatusEffect.StatusEffectType.None)
            {
                // Debug.LogWarning("no status effect assigned!");
                return;
            }

            if (TryGetComponent(out StatusEffect preExistingEffect) && statusEffect.type == preExistingEffect.type)
            {
                preExistingEffect.SetEffectVars(statusEffect);
                RemoveFromTextUI(statusEffect);
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

            CheckEffectInteraction(statusEffect);
        }

        private void CheckEffectInteraction(StatusEffect statusEffect)
        {
            if (statusEffect.type == StatusEffect.StatusEffectType.Wet)
            {
                for (int i = 0; i < statusEffectList.Count; i++)
                {
                    if (statusEffectList[i].type == StatusEffect.StatusEffectType.Burning)
                    {
                        RemoveStatusEffect(i--);
                    }
                }
            }
            else if (statusEffect.type == StatusEffect.StatusEffectType.Burning)
            {
                for (int i = 0; i < statusEffectList.Count; i++)
                {
                    if (statusEffectList[i].type == StatusEffect.StatusEffectType.Wet)
                    {
                        RemoveStatusEffect(i--);
                    }
                }
            }
        }

        private void AddToTextUI(StatusEffect statusEffect)
        {
            if (textUI.text != "")
            {
                textUI.text += $"\n";
            }

            if (statusEffect.type == StatusEffect.StatusEffectType.Burning)
            {
                burningText = $"{statusEffect.type.ToString()} damages {statusEffect.effectAmount} vitality for {statusEffect.cooldownTimer} more turn(s)";
                textUI.text += burningText;
            }
            else if (statusEffect.type == StatusEffect.StatusEffectType.Regenerating)
            {
                regeneratingText = $"{statusEffect.type.ToString()} heals {statusEffect.effectAmount} vitality for {statusEffect.cooldownTimer} more turn(s)";
                textUI.text += regeneratingText;
            }
            else if (statusEffect.type == StatusEffect.StatusEffectType.MagicShell)
            {
                magicShellText = $"{statusEffect.type.ToString()} heals {statusEffect.effectAmount} vitality for {statusEffect.cooldownTimer} more turn(s)";
                textUI.text += magicShellText;
            }
            else if (statusEffect.effectApplication == StatusEffect.StatusEffectApplication.Wet)
            {
                wetText = $"{statusEffect.type.ToString()}! for {statusEffect.cooldownTimer} more turn(s)";
                textUI.text += wetText;
            }
            else if (statusEffect.effectApplication == StatusEffect.StatusEffectApplication.Slow)
            {
                slowText = $"{statusEffect.type.ToString()}! for {statusEffect.cooldownTimer} more turn(s)";
                textUI.text += slowText;
            }
            else if (statusEffect.effectApplication == StatusEffect.StatusEffectApplication.RestorePhysicalArmor)
            {
                textUI.text += $"{statusEffect.type.ToString()} increases magic armor by {statusEffect.effectAmount} for {statusEffect.cooldownTimer} more turn(s)";
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

        private void RemoveFromTextUI(StatusEffect effect)
        {
            if (effect.effectApplication == StatusEffect.StatusEffectApplication.Damage)
            {
                textUI.text = textUI.text.Replace(burningText, string.Empty);
            }
            else if (effect.effectApplication == StatusEffect.StatusEffectApplication.Wet)
            {
                textUI.text = textUI.text.Replace(wetText, string.Empty);
            }
            else if (effect.effectApplication == StatusEffect.StatusEffectApplication.Slow)
            {
                textUI.text = textUI.text.Replace(slowText, string.Empty);
            }

            textUI.text = textUI.text.Replace(Environment.NewLine, "");
        }
    }
}