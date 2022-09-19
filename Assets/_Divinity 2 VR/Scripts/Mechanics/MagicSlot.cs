using System;
using HighlightPlus;
using HurricaneVR.Framework.Core.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace intheclouds
{
    public class MagicSlot : MonoBehaviour
    {
        public bool dequipMagic;
        public Magic magic;
        public AudioClip onSelectedAudioClip;
        public Color handAugmentColor;
        private MagicSystem magicSystem;
        public GameObject readyArt;
        public GameObject cooldownArt;

        private void Awake()
        {
            magicSystem = transform.parent.parent.GetComponent<MagicSystem>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Left Hand") && !magicSystem.spawnedMagic)
            {
                if (dequipMagic && magicSystem.selectedMagic)
                {
                    magicSystem.DequipMagic();
                    SFXPlayer.Instance.PlaySFXAttach(onSelectedAudioClip, other.transform, 1, 0.3f);
                    return;
                }

                if (magic && magicSystem.selectedMagic != magic)
                {
                    if (magicSystem.selectedMagic) // dequip current ability if selected different ability
                    {
                        magicSystem.DequipMagic();
                    }

                    magicSystem.selectedMagic = magic;
                    magicSystem.selectedMagic.magicSystem = magicSystem;
                    magicSystem.selectedMagic.magicSlot = this;
                    magicSystem.playerLUOs.handAugmentHighlight.overlayColor = handAugmentColor;
                    magicSystem.playerLUOs.handAugmentHighlight.SetGlowColor(handAugmentColor);
                    if (onSelectedAudioClip)
                    {
                        SFXPlayer.Instance.PlaySFXAttach(onSelectedAudioClip, other.transform, 1, 1);
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
            if (magicSystem.description.transform.childCount > 0)
            {
                foreach (Transform child in magicSystem.description.transform)
                {
                    if (child.name == magic.GetComponent<Magic>().abilityDescription.name + "(Clone)")
                    {
                        child.gameObject.SetActive(true);
                        // todo: check if selected magic damage is same as damage text. if not, then replace old description with a new one
                        return;
                    }
                }
            }

            var description = Instantiate(magic.GetComponent<Magic>().abilityDescription, magicSystem.description.transform);
            if (magicSystem.description.transform.childCount == 1)
            {
                magicSystem.description.SetActive(true);
                foreach (Transform child in description.transform)
                {
                    if (child.name == "Damage text")
                    {
                        Debug.Log("updating damage text");
                        int scaledDamageUI = magicSystem.selectedMagic.amount;
                        if (magicSystem.selectedMagic.elementalType == ElementalType.Fire)
                        {
                            scaledDamageUI *=  magicSystem.playerLUOs.PlayerStats.level * (1 + magicSystem.playerLUOs.PlayerStats.Pyrokinetic);
                        }
                        else if (magicSystem.selectedMagic.elementalType == ElementalType.Water)
                        {
                            scaledDamageUI *= magicSystem.playerLUOs.PlayerStats.level * (1 + magicSystem.playerLUOs.PlayerStats.Hydrosophist);
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