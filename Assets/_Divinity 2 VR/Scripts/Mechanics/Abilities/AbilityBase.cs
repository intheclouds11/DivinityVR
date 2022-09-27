using HurricaneVR.Framework.Components;
using HurricaneVR.Framework.Core.Grabbers;
using HurricaneVR.Framework.Shared;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace intheclouds
{
    public class AbilityBase : MonoBehaviour
    {
        public int cooldown;
        public int cooldownTimer;
        public int amount;
        public int requiredAP;
        public GameObject abilityDescription;
        public GameObject surfaceEffect;
        public StatusEffect statusEffect;
        public ElementalType elementalType;
        public GameObject activatedVFX;
        public AudioClip noDamageAudioClip;
        [Header("Debug")]
        public AbilitySystem abilitySystem;
        public AbilitySlot abilitySlot;
        public PlayerStats caster;
        public HVRHandSide castingHand;

        private void OnEnable()
        {
            if (amount != 0)
            {
                ApplyScaling();
            }
        }

        private void ApplyScaling()
        {
            if (elementalType == ElementalType.Fire)
            {
                amount *= abilitySystem.playerLUOs.PlayerStats.level * (1 + abilitySystem.playerLUOs.PlayerStats.Pyrokinetic);
            }
            else if (elementalType == ElementalType.Water)
            {
                amount *= abilitySystem.playerLUOs.PlayerStats.level * (1 + abilitySystem.playerLUOs.PlayerStats.Hydrosophist);
            }

            Debug.Log($"updated {name} amount based on player stats");
        }

        public void OnAbilityUsed()
        {
            activatedVFX.transform.parent = null;
            activatedVFX.SetActive(true);

            if (castingHand == HVRHandSide.Left)
            {
                caster.LocalUserObjects.leftHandPhysics.GetComponent<HVRHandGrabber>().ForceRelease();
            }
            else
            {
                caster.LocalUserObjects.rightHandPhysics.GetComponent<HVRHandGrabber>().ForceRelease();
            }

            // var selectedAbility = abilitySystem.selectedAbility.GetComponent<AbilityBase>();
            abilitySystem.DequipAbility();
            cooldownTimer = cooldown;
            abilitySlot.readyArt.SetActive(false);
            abilitySlot.cooldownArt.SetActive(true);
            abilitySlot.cooldownArt.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"Cooldown: {cooldown}";

            gameObject.SetActive(false);
            transform.parent = caster.LocalUserObjects.abilities.transform;
            transform.position = caster.LocalUserObjects.abilities.transform.position;
            transform.rotation = caster.LocalUserObjects.abilities.transform.rotation;
        }

        public void OnAbilityReady()
        {
            activatedVFX.transform.parent = transform;
            activatedVFX.transform.position = transform.position;
            activatedVFX.transform.rotation = transform.rotation;
            activatedVFX.SetActive(false);

            abilitySlot.readyArt.SetActive(true);
            abilitySlot.cooldownArt.SetActive(false);
        }
    }
}