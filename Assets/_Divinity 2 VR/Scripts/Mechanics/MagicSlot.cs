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
        private MagicSystem magicSelector;

        private void Awake()
        {
            magicSelector = transform.parent.parent.GetComponent<MagicSystem>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Left Hand"))
            {
                if (dequipMagic && magicSelector.selectedMagic)
                {
                    magicSelector.selectedMagic.SetActive(false);
                    magicSelector.selectedMagic = null;
                    Destroy(magicSelector.description.transform.GetChild(0).gameObject);
                    SFXPlayer.Instance.PlaySFXAttach(onSelectedAudioClip, other.transform, 1, 0.3f);
                    return;
                }

                if (magic != null && magicSelector.selectedMagic != magic)
                {
                    magicSelector.selectedMagic = magic;
                    magicSelector.playerLUOs.handAugmentHighlight.overlayColor = handAugmentColor;
                    magicSelector.playerLUOs.handAugmentHighlight.SetGlowColor(handAugmentColor);
                    if (onSelectedAudioClip)
                    {
                        SFXPlayer.Instance.PlaySFXAttach(onSelectedAudioClip, other.transform, 1, 1);
                    }

                    Instantiate(magic.GetComponent<Magic>().abilityDescription, magicSelector.description.transform);
                    if (magicSelector.description.transform.childCount == 1)
                    {
                        magicSelector.description.SetActive(true);
                    }
                }
            }
        }
    }
}