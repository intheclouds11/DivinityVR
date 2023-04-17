using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace intheclouds
{
    public class StatusEffectsContainer : MonoBehaviour
    {
        public List<StatusEffect> statusEffectList;
        public int explorationCooldownSpeed = 4;
        private float cooldownTimerNoCombat;
        private BaseStats combatant;
        private TextMeshProUGUI textUI;
        private string burningText;
        private string regeneratingText;
        private string wetText;
        private string slowText;
        private string stunnedText;
        private string magicShellText;

        private void Start()
        {
            combatant = GetComponentInParent<BaseStats>();
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
            if (cooldownTimerNoCombat < explorationCooldownSpeed)
            {
                cooldownTimerNoCombat += Time.deltaTime;
            }
            else if (cooldownTimerNoCombat >= explorationCooldownSpeed)
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

        public void TryAddStatusEffect(StatusEffect effect)
        {
            if (!effect || effect.type == StatusEffectType.None)
            {
                // Debug.LogWarning("no status effect assigned!");
                return;
            }


            int applyChanceRange = new System.Random().Next(1, 101);
            if (effect.ChanceToApply > 0 && effect.ChanceToApply < applyChanceRange)
            {
                Debug.Log($"effect.ChanceToApply {effect.ChanceToApply} < applyChanceRange {applyChanceRange}");
                return;
            }

            if (TryGetComponent(out StatusEffect preExistingEffect) && effect.type == preExistingEffect.type)
            {
                preExistingEffect.StatusEffectConstructor(effect, true);
                RemoveFromTextUI(effect);
                AddToTextUI(effect);
                Debug.Log("status effect reapplied");
            }
            else
            {
                var appliedEffect = gameObject.AddComponent<StatusEffect>();
                appliedEffect.StatusEffectConstructor(effect);
                statusEffectList.Add(appliedEffect);
                AddToTextUI(effect);
                Debug.Log("status effect applied (first time)");
            }

            CheckEffectInteraction(effect);
        }

        private void CheckEffectInteraction(StatusEffect statusEffect)
        {
            if (statusEffect.type is StatusEffectType.Wet or StatusEffectType.Regenerating)
            {
                for (int i = 0; i < statusEffectList.Count; i++)
                {
                    if (statusEffectList[i].type == StatusEffectType.Burning)
                    {
                        RemoveStatusEffect(i--);
                    }
                }
            }
            else if (statusEffect.type == StatusEffectType.Burning)
            {
                for (int i = 0; i < statusEffectList.Count; i++)
                {
                    if (statusEffectList[i].type == StatusEffectType.Wet)
                    {
                        RemoveStatusEffect(i--);
                    }
                }
            }
        }

        private void AddToTextUI(StatusEffect effect)
        {
            if (textUI.text != "")
            {
                textUI.text += $"\n";
            }

            if (effect.type == StatusEffectType.Burning)
            {
                burningText = $"{effect.type.ToString()} damages {effect.effectAmount} vitality for {effect.cooldownTimer} more turn(s)";
                textUI.text += burningText;
            }
            else if (effect.type == StatusEffectType.Regenerating)
            {
                regeneratingText = $"{effect.type.ToString()} heals {effect.effectAmount} vitality for {effect.cooldownTimer} more turn(s)";
                textUI.text += regeneratingText;
            }
            else if (effect.type == StatusEffectType.MagicShell)
            {
                magicShellText = $"{effect.type.ToString()} restores {effect.effectAmount} magic armor for {effect.cooldownTimer} more turn(s)";
                textUI.text += magicShellText;
            }
            else if (effect.type == StatusEffectType.Wet)
            {
                wetText = $"{effect.type.ToString()}! for {effect.cooldownTimer} more turn(s)";
                textUI.text += wetText;
            }
            else if (effect.type == StatusEffectType.Slowed)
            {
                slowText = $"{effect.type.ToString()}! for {effect.cooldownTimer} more turn(s)";
                textUI.text += slowText;
            }
            else if (effect.type == StatusEffectType.Stunned)
            {
                stunnedText = $"{effect.type.ToString()}! for {effect.cooldownTimer} more turn(s)";
                textUI.text += stunnedText;
            }
        }

        private void RemoveFromTextUI(StatusEffect effect)
        {
            if (effect.type == StatusEffectType.Burning)
            {
                textUI.text = textUI.text.Replace(burningText, string.Empty);
            }
            else if (effect.type == StatusEffectType.Wet)
            {
                textUI.text = textUI.text.Replace(wetText, string.Empty);
            }
            else if (effect.type == StatusEffectType.Slowed)
            {
                textUI.text = textUI.text.Replace(slowText, string.Empty);
            }
            else if (effect.type == StatusEffectType.Regenerating)
            {
                textUI.text = textUI.text.Replace(regeneratingText, string.Empty);
            }
            else if (effect.type == StatusEffectType.Stunned)
            {
                textUI.text = textUI.text.Replace(stunnedText, string.Empty);
            }

            textUI.text = textUI.text.Replace(Environment.NewLine, "");
        }
    }
}