using System;
using HurricaneVR.Framework.Core.Utils;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace intheclouds
{
    public class PlayerStats : MonoBehaviour
    {
        public PlayerStatsSO playerStatsSO;
        public string userName = "Username";
        public Slider healthSlider;
        public Slider physicalArmorSlider;
        public Slider magicArmorSlider;
        public Slider apSlider;
        public Slider xpSlider;
        public TextMeshProUGUI healthText;
        public TextMeshProUGUI physicalArmorText;
        public TextMeshProUGUI magicArmorText;
        public TextMeshProUGUI apText;
        public TextMeshProUGUI goldText;
        public int currentHealth;
        public int maxHealth;
        public int currentPhysicalArmor;
        public int maxPhysicalArmor;
        public int currentMagicArmor;
        public int maxMagicArmor;
        public int currentAP;
        public int maxAP;
        public int gold;
        public int XP;
        public int XPToNextLevel;
        public AudioClip levelUpClip;
        public bool playerTurnCombat;
        public bool explorationMode = true;

        public event Action PlayerDamaged; // use for other classes to know when player is damaged

        private void OnEnable()
        {
            InitializeStats();
        }

        public void ToggleExplorationMode()
        {
            explorationMode = !explorationMode;
        }

        private void Update()
        {
            UpdateStatsHud();
            if (currentAP == 0)
            {
                playerTurnCombat = false;
            }
        }

        private void InitializeStats()
        {
            userName = playerStatsSO.userName;

            maxHealth = playerStatsSO.maxHealth;
            currentHealth = playerStatsSO.currentHealth;

            maxPhysicalArmor = playerStatsSO.maxPhysicalArmor;
            currentPhysicalArmor = playerStatsSO.currentPhysicalArmor;

            maxMagicArmor = playerStatsSO.maxMagicArmor;
            currentMagicArmor = playerStatsSO.currentMagicArmor;

            maxAP = playerStatsSO.maxAP;
            currentAP = playerStatsSO.currentAP;
            XP = playerStatsSO.XP;
            XPToNextLevel = playerStatsSO.XPToNextLevel;
            gold = playerStatsSO.gold;
        }

        private void UpdateStatsHud()
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
            healthText.text = $"{currentHealth}/{maxHealth}";

            physicalArmorSlider.maxValue = maxPhysicalArmor;
            physicalArmorSlider.value = currentPhysicalArmor;
            physicalArmorText.text = $"{currentPhysicalArmor}/{maxPhysicalArmor}";

            magicArmorSlider.maxValue = maxMagicArmor;
            magicArmorSlider.value = currentMagicArmor;
            magicArmorText.text = $"{currentMagicArmor}/{maxMagicArmor}";

            apSlider.maxValue = maxAP;
            apSlider.value = currentAP;
            apText.text = $"{currentAP}/{maxAP}";

            xpSlider.maxValue = XPToNextLevel;
            xpSlider.value = XP;
            goldText.text = $"Gold: {gold}";
        }

        public void TakeDamage(int damage)
        {
            PlayerDamaged?.Invoke();
            currentHealth -= damage;
            healthSlider.value = currentHealth;
            if (currentHealth <= 0)
            {
                Debug.Log("player died!!!");
                // GameManager.Instance.UpdateGameState(GameState.Lose);
            }
        }

        public void UseAP(int apConsumed)
        {
            currentAP -= apConsumed;
            apSlider.value = currentAP;
        }

        public void ObtainXP(int xp)
        {
            Debug.Log("get xp");
            XP += xp;

            if (XP > XPToNextLevel)
            {
                LevelUp();
                var xpGainDelta = XP - XPToNextLevel;

                if (xpGainDelta > XPToNextLevel)
                {
                    LevelUp();
                }
            }
        }

        public void LevelUp()
        {
            XPToNextLevel += (int) (XPToNextLevel * 0.5f);
            SFXPlayer.Instance.PlaySFX(levelUpClip, transform);
            // award 1 Attribute Point, 1 Skill Point, 1 Talent
        }

        public void SaveProgress()
        {
            playerStatsSO.userName = userName;

            playerStatsSO.maxHealth = maxHealth;
            playerStatsSO.currentHealth = currentHealth;

            playerStatsSO.maxPhysicalArmor = maxPhysicalArmor;
            playerStatsSO.currentPhysicalArmor = currentPhysicalArmor;

            playerStatsSO.maxMagicArmor = maxMagicArmor;
            playerStatsSO.currentMagicArmor = currentMagicArmor;

            playerStatsSO.maxAP = maxAP;
            playerStatsSO.currentAP = currentAP;
            playerStatsSO.XP = XP;
            playerStatsSO.gold = gold;
        }
    }
}