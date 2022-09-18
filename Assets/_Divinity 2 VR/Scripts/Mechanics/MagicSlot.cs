using System;
using HighlightPlus;
using HurricaneVR.Framework.Core.Utils;
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
                    if (magicSystem.selectedMagic)
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

                    Instantiate(magic.GetComponent<Magic>().skillDescription, magicSystem.description.transform);
                    if (magicSystem.description.transform.childCount == 1)
                    {
                        magicSystem.description.SetActive(true);
                    }
                }
                
                if (readyArt && readyArt.activeSelf && magic)
                {
                    readyArt.GetComponent<HighlightEffect>().highlighted = true;
                }
            }
        }
    }
}