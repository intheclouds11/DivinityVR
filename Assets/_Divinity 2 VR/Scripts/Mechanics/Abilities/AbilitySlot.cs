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
        public bool dequipMagic;
        public AbilityBase magic;
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
            if (!abilitySystem.spawnedMagic && other.gameObject.CompareTag("PointerFingerTip"))
            {
                if (dequipMagic && abilitySystem.selectedMagic)
                {
                    abilitySystem.DequipMagic();
                    SFXPlayer.Instance.PlaySFX(onSelectedAudioClip, other.transform.position, 1, 0.3f);
                    return;
                }

                if (magic && abilitySystem.selectedMagic != magic)
                {
                    if (abilitySystem.selectedMagic) // dequip current ability if selected different ability
                    {
                        abilitySystem.DequipMagic();
                    }

                    abilitySystem.selectedMagic = magic;
                    abilitySystem.selectedMagic.magicSystem = abilitySystem;
                    abilitySystem.selectedMagic.magicSlot = this;
                    abilitySystem.playerLUOs.handAugmentHighlight.overlayColor = handAugmentColor;
                    abilitySystem.playerLUOs.handAugmentHighlight.SetGlowColor(handAugmentColor);
                    if (onSelectedAudioClip)
                    {
                        SFXPlayer.Instance.PlaySFX(onSelectedAudioClip, other.transform.position, 1, 1);
                    }

                    SpawnDescription();
                }

                if (readyArt && readyArt.activeSelf && magic)
                {
                    readyArt.GetComponent<HighlightEffect>().highlighted = true;
                }
            }
        }

        private void SpawnDescription()
        {
            // More performant than destroying, but annoying to keep up with
            // if (magicSystem.description.transform.childCount > 0)
            // {
            //     foreach (Transform child in magicSystem.description.transform)
            //     {
            //         if (child.name == magic.GetComponent<Magic>().abilityDescription.name + "(Clone)")
            //         {
            //             child.gameObject.SetActive(true);
            //             // need to check if selected magic damage is same as damage text. if not, then replace old description with a new one
            //             return;
            //         }
            //     }
            // }

            var description = Instantiate(magic.GetComponent<AbilityBase>().abilityDescription, abilitySystem.description.transform);
            if (abilitySystem.description)
            {
                abilitySystem.description.SetActive(true);
                foreach (Transform child in description.transform)
                {
                    if (child.name == "Damage text")
                    {
                        int scaledDamageUI = abilitySystem.selectedMagic.amount;
                        if (abilitySystem.selectedMagic.elementalType == ElementalType.Fire)
                        {
                            scaledDamageUI *=  abilitySystem.playerLUOs.PlayerStats.level * (1 + abilitySystem.playerLUOs.PlayerStats.Pyrokinetic);
                        }
                        else if (abilitySystem.selectedMagic.elementalType == ElementalType.Water)
                        {
                            scaledDamageUI *= abilitySystem.playerLUOs.PlayerStats.level * (1 + abilitySystem.playerLUOs.PlayerStats.Hydrosophist);
                        }
                        var damageText = child.GetComponent<TextMeshProUGUI>();
                        var low = (int) Math.Floor(scaledDamageUI * 0.15f);
                        if (low == 0)
                        {
                            low = 1;
                        }

                        var high = (int) Math.Ceiling(scaledDamageUI * 0.15f);
                        if (high == 0)
                        {
                            high = 1;
                        }

                        damageText.text = damageText.text.Replace("[damage]", $"{scaledDamageUI - low} - {scaledDamageUI + high}");
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