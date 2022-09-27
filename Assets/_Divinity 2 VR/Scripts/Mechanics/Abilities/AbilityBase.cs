using System;
using System.Collections;
using System.Collections.Generic;
using HurricaneVR.Framework.Core;
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
        public AbilitySystem magicSystem;
        public AbilitySlot magicSlot;
        public PlayerStats caster;

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
                amount *= magicSystem.playerLUOs.PlayerStats.level * (1 + magicSystem.playerLUOs.PlayerStats.Pyrokinetic);
            }
            else if (elementalType == ElementalType.Water)
            {
                amount *= magicSystem.playerLUOs.PlayerStats.level * (1 + magicSystem.playerLUOs.PlayerStats.Hydrosophist);
            }

            Debug.Log($"updated {name} amount based on player stats");
        }

        protected void OnMagicUsed()
        {
            var selectedMagic = magicSystem.selectedMagic.GetComponent<AbilityBase>();
            selectedMagic.magicSystem.DequipMagic();
            selectedMagic.cooldownTimer = cooldown;
            magicSlot.readyArt.SetActive(false);
            magicSlot.cooldownArt.SetActive(true);
            magicSlot.cooldownArt.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"Cooldown: {cooldown}";
        }

        public void OnMagicReady()
        {
            magicSlot.readyArt.SetActive(true);
            magicSlot.cooldownArt.SetActive(false);
        }
    }
}