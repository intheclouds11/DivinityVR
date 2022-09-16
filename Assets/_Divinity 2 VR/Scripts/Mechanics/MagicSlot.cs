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

        private void Awake()
        {
            magicSystem = transform.parent.parent.GetComponent<MagicSystem>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Left Hand"))
            {
                if (dequipMagic && magicSystem.selectedMagic)
                {
                    magicSystem.DequipMagic();
                    SFXPlayer.Instance.PlaySFXAttach(onSelectedAudioClip, other.transform, 1, 0.3f);
                    return;
                }

                if (magic != null && magicSystem.selectedMagic != magic)
                {
                    magicSystem.selectedMagic = magic;
                    magicSystem.selectedMagic.magicSystem = magicSystem;
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
            }
        }
    }
}