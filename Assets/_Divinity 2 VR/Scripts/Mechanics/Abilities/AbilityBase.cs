using System;
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
        public AudioClip activatedAudioClip;
        [Header("Debug")]
        public AbilitySystem abilitySystem;
        public AbilitySlot abilitySlot;
        public PlayerStats caster;
        public HVRHandSide castingHand;

        protected virtual void OnEnable()
        {
            if (amount != 0)
            {
                ApplyScaling();
            }
        }

        protected virtual void OnDisable()
        {
        }

        protected virtual void ApplyScaling()
        {
            if (elementalType == ElementalType.Fire)
            {
                amount *= abilitySystem.playerLUOs.PlayerStats.level * (1 + abilitySystem.playerLUOs.PlayerStats.Pyrokinetic);
            }
            else if (elementalType == ElementalType.Water)
            {
                amount *= abilitySystem.playerLUOs.PlayerStats.level * (1 + abilitySystem.playerLUOs.PlayerStats.Hydrosophist);
            }
            else if (elementalType == ElementalType.Earth)
            {
                amount *= abilitySystem.playerLUOs.PlayerStats.level * (1 + abilitySystem.playerLUOs.PlayerStats.Geomancer);
            }

            Debug.Log($"updated {name} amount based on player stats");
        }

        protected virtual void OnAbilityUsed()
        {
            if (activatedVFX != null)
            {
                activatedVFX.transform.parent = null;
                activatedVFX.SetActive(true);
            }

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

            ResetAbilityTransform();
        }

        protected void ResetAbilityTransform()
        {
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