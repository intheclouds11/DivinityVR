using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace intheclouds
{
    public class Magic : MonoBehaviour
    {
        public int cooldown;
        public int cooldownTimer;
        public int amount;
        public int requiredAP;
        public GameObject abilityDescription;
        public GameObject surfaceEffect;
        public StatusEffect statusEffect;
        public ElementalType elementalType;
        public GameObject impactVFX;
        public AudioClip noDamageAudioClip;
        [Header("Debug")]
        public MagicSystem magicSystem;
        public MagicSlot magicSlot;
        public PlayerStats caster;

        private void OnEnable()
        {
            ApplyScaling();
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
            var selectedMagic = magicSystem.selectedMagic.GetComponent<Magic>();
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