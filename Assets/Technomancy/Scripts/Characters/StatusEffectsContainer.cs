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
        private float _cooldownTimerNoCombat;
        private BaseStats _combatant;
        private TextMeshProUGUI _textUI;
        private string _burningText;
        private string _regeneratingText;
        private string _wetText;
        private string _slowText;
        private string _stunnedText;
        private string _magicShellText;

        private void Start()
        {
            _combatant = GetComponentInParent<BaseStats>();
            _textUI = _combatant.statusEffectsText;
        }

        void Update()
        {
            if (!_combatant.InCombat)
            {
                StatusEffectCooldownExploration();
            }
        }

        public void StatusEffectCooldownExploration()
        {
            if (_cooldownTimerNoCombat < explorationCooldownSpeed)
            {
                _cooldownTimerNoCombat += Time.deltaTime;
            }
            else if (_cooldownTimerNoCombat >= explorationCooldownSpeed)
            {
                StatusEffectCooldown();
                _cooldownTimerNoCombat = 0;
            }
        }

        public void StatusEffectCooldown()
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
            if (effect.ChanceToApply > 0 && applyChanceRange <= effect.ChanceToApply)
            {
                // Debug.Log($"effect.ChanceToApply {effect.ChanceToApply} < applyChanceRange {applyChanceRange}");
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
            else if (statusEffect.type == StatusEffectType.Fortify)
            {
                for (int i = 0; i < statusEffectList.Count; i++)
                {
                    if (statusEffectList[i].type == StatusEffectType.Burning)
                    {
                        RemoveStatusEffect(i--);
                    }
                }
            }
        }

        private void AddToTextUI(StatusEffect effect)
        {
            if (_textUI.text != "")
            {
                _textUI.text += $"\n";
            }

            if (effect.type == StatusEffectType.Burning)
            {
                _burningText = $"{effect.type.ToString()} damages {effect.effectAmount} vitality for {effect.cooldownTimer} more turn(s)";
                _textUI.text += _burningText;
            }
            else if (effect.type == StatusEffectType.Regenerating)
            {
                _regeneratingText = $"{effect.type.ToString()} heals {effect.effectAmount} vitality for {effect.cooldownTimer} more turn(s)";
                _textUI.text += _regeneratingText;
            }
            else if (effect.type == StatusEffectType.MagicShell)
            {
                _magicShellText = $"{effect.type.ToString()} restores {effect.effectAmount} magic armor for {effect.cooldownTimer} more turn(s)";
                _textUI.text += _magicShellText;
            }
            else if (effect.type == StatusEffectType.Wet)
            {
                _wetText = $"{effect.type.ToString()}! for {effect.cooldownTimer} more turn(s)";
                _textUI.text += _wetText;
            }
            else if (effect.type == StatusEffectType.Slowed)
            {
                _slowText = $"{effect.type.ToString()}! for {effect.cooldownTimer} more turn(s)";
                _textUI.text += _slowText;
            }
            else if (effect.type == StatusEffectType.Stunned)
            {
                _stunnedText = $"{effect.type.ToString()}! for {effect.cooldownTimer} more turn(s)";
                _textUI.text += _stunnedText;
            }
        }

        private void RemoveFromTextUI(StatusEffect effect)
        {
            if (effect.type == StatusEffectType.Burning)
            {
                _textUI.text = _textUI.text.Replace(_burningText, string.Empty);
            }
            else if (effect.type == StatusEffectType.Wet)
            {
                _textUI.text = _textUI.text.Replace(_wetText, string.Empty);
            }
            else if (effect.type == StatusEffectType.Slowed)
            {
                _textUI.text = _textUI.text.Replace(_slowText, string.Empty);
            }
            else if (effect.type == StatusEffectType.Regenerating)
            {
                _textUI.text = _textUI.text.Replace(_regeneratingText, string.Empty);
            }
            else if (effect.type == StatusEffectType.Stunned)
            {
                _textUI.text = _textUI.text.Replace(_stunnedText, string.Empty);
            }

            _textUI.text = _textUI.text.Replace(Environment.NewLine, "");
        }
    }
}