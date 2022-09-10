using System;
using System.Collections;
using System.Collections.Generic;
using HurricaneVR.Framework.Core.Utils;
using UnityEngine;
using UnityEngine.Serialization;

namespace intheclouds
{
    public class MagicSlot : MonoBehaviour
    {
        public bool dequipMagic;
        public GameObject magic;
        public AudioClip onSelectedAudioClip;
        public Color handAugmentColor;
        private MagicSelector magicSelector;

        private void Awake()
        {
            magicSelector = transform.parent.GetComponent<MagicSelector>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Left Hand"))
            {
                if (dequipMagic && magicSelector.selectedMagic)
                {
                    magicSelector.selectedMagic.SetActive(false);
                    magicSelector.selectedMagic = null;
                    SFXPlayer.Instance.PlaySFXAttach(onSelectedAudioClip, other.transform, 1, 0.3f);
                    return;
                }

                if (magic)
                {
                    magic.SetActive(true);
                    magicSelector.selectedMagic = magic;
                    magicSelector.player.LocalUserObjects.handAugmentHighlight.overlayColor = handAugmentColor;
                    magicSelector.player.LocalUserObjects.handAugmentHighlight.SetGlowColor(handAugmentColor);
                    magicSelector.Deactivate();
                    SFXPlayer.Instance.PlaySFXAttach(onSelectedAudioClip, other.transform, 1, 1);
                }
            }
        }
    }
}