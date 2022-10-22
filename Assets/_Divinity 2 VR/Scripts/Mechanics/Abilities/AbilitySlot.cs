using System;
using HighlightPlus;
using HurricaneVR.Framework.Core.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace intheclouds
{
    public class AbilitySlot : MonoBehaviour
    {
        public bool dequipAbility;
        public AbilityBase ability;
        public AudioClip onSelectedAudioClip;
        public Color handAugmentColor;
        private AbilitySystem abilitySystem;
        public GameObject readyArt;
        public GameObject cooldownArt;

        private void Awake()
        {
            abilitySystem = transform.parent.parent.GetComponent<AbilitySystem>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("PointerFingerTip") && (!abilitySystem.selectedAbility || !abilitySystem.selectedAbility.gameObject.activeInHierarchy))
            {
                if (dequipAbility && abilitySystem.selectedAbility)
                {
                    abilitySystem.DequipAbility();
                    SFXPlayer.Instance.PlaySFX(onSelectedAudioClip, other.transform.position, 1, 0.3f);
                    return;
                }

                if (ability && abilitySystem.selectedAbility != ability)
                {
                    if (abilitySystem.selectedAbility) // dequip current ability if selected different ability
                    {
                        abilitySystem.DequipAbility();
                    }

                    abilitySystem.selectedAbility = ability;
                    abilitySystem.selectedAbility.abilitySystem = abilitySystem;
                    abilitySystem.selectedAbility.abilitySlot = this;
                    abilitySystem.playerLUOs.handAugmentHighlight.overlayColor = handAugmentColor;
                    abilitySystem.playerLUOs.handAugmentHighlight.SetGlowColor(handAugmentColor);
                    if (onSelectedAudioClip)
                    {
                        SFXPlayer.Instance.PlaySFX(onSelectedAudioClip, other.transform.position, 1, 1);
                    }

                    SpawnDescription();
                }

                if (readyArt && readyArt.activeSelf && ability)
                {
                    readyArt.GetComponent<HighlightEffect>().highlighted = true;
                }
            }
        }

        private void SpawnDescription()
        {
            var description = Instantiate(ability.GetComponent<AbilityBase>().abilityDescription, abilitySystem.description.transform);
            if (abilitySystem.description)
            {
                abilitySystem.description.SetActive(true);
                foreach (Transform child in description.transform)
                {
                    if (child.name == "Damage text")
                    {
                        var damageText = child.GetComponent<TextMeshProUGUI>();
                        var low = (int) Math.Floor(abilitySystem.selectedAbility.scaledAmount * 0.15f);
                        if (low == 0)
                        {
                            low = 1;
                        }

                        var high = (int) Math.Ceiling(abilitySystem.selectedAbility.scaledAmount * 0.15f);
                        if (high == 0)
                        {
                            high = 1;
                        }

                        damageText.text = damageText.text.Replace("[damage]",
                            $"{abilitySystem.selectedAbility.scaledAmount - low} - {abilitySystem.selectedAbility.scaledAmount + high}");
                        break;
                    }

                    if (child.name == "Healing text")
                    {
                        Debug.Log("TODO: update healing description");
                        break;
                    }
                }
            }
            else
            {
                Debug.LogError("No ability description found!");
            }
        }
    }
}