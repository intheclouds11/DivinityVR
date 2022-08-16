using System;
using TMPro;
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
        public TextMeshProUGUI healthText;
        public TextMeshProUGUI physicalArmorText;
        public TextMeshProUGUI magicArmorText;
        public TextMeshProUGUI apText;
        public int currentHealth;
        public int maxHealth;
        public int currentPhysicalArmor;
        public int maxPhysicalArmor;
        public int currentMagicArmor;
        public int maxMagicArmor;
        public int currentAP;
        public int maxAP;
        public bool playerTurnCombat;
        public bool explorationMode = true;
        
        public event Action Damaged; // use for other classes to know when player is damaged

        private void Awake()
        {
            InitializeStats();
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
        }

        public void TakeDamage(int damage)
        {
            Damaged?.Invoke();
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
            apSlider.value -= apConsumed;
            apText.text = $"{currentAP}/{maxAP}";
        }
    }
}