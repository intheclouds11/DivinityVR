using System;
using System.Diagnostics.CodeAnalysis;
using HurricaneVR.Framework.Core.Utils;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace intheclouds
{
    [SuppressMessage("ReSharper", "ArrangeAccessorOwnerBody")]
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
        public AudioClip levelUpClip;

        [Tooltip("Player's Stats")]
        private int _currentHealth;
        public int currentHealth
        {
            get { return _currentHealth; }
            set
            {
                _currentHealth = value;
                UpdateHealthInfo();
            }
        }
        private int _maxHealth;
        public int maxHealth
        {
            get => _maxHealth;
            set
            {
                _maxHealth = value;
                UpdateHealthInfo();
            }
        }
        private int _currentPhysicalArmor;
        public int currentPhysicalArmor
        {
            get { return _currentPhysicalArmor; }
            set
            {
                _currentPhysicalArmor = value;
                UpdatePhysicalArmorInfo();
            }
        }
        private int _maxPhysicalArmor;
        public int maxPhysicalArmor
        {
            get { return _maxPhysicalArmor; }
            set
            {
                _maxPhysicalArmor = value;
                UpdatePhysicalArmorInfo();
            }
        }
        private int _currentMagicArmor;
        public int currentMagicArmor
        {
            get { return _currentMagicArmor; }
            set
            {
                _currentMagicArmor = value;
                UpdateMagicArmorInfo();
            }
        }
        private int _maxMagicArmor;
        public int maxMagicArmor
        {
            get { return _maxMagicArmor; }
            set
            {
                _maxMagicArmor = value;
                UpdateMagicArmorInfo();
            }
        }
        private int _currentAP;
        public int currentAP
        {
            get { return _currentAP; }
            set
            {
                _currentAP = value;
                UpdateAPInfo();
                if (_currentAP == 0)
                {
                    playerTurnCombat = false;
                }
            }
        }
        private int _maxAP;
        public int maxAP
        {
            get { return _maxAP; }
            set
            {
                _maxAP = value;
                UpdateAPInfo();
            }
        }
        private int _gold;
        public int gold
        {
            get { return _gold; }
            set
            {
                _gold = value;
                UpdateGoldInfo();
            }
        }
        private int _XP;
        public int XP
        {
            get { return _XP; }
            set
            {
                _XP = value;
                UpdateXPInfo();
            }
        }
        private int _XPToNextLevel;
        public int XPToNextLevel
        {
            get { return _XPToNextLevel; }
            set
            {
                _XPToNextLevel = value;
                UpdateXPInfo();
            }
        }
        private bool _playerTurnCombat;
        public bool playerTurnCombat
        {
            get { return _playerTurnCombat; }
            set
            {
                _playerTurnCombat = value;
                if (_playerTurnCombat)
                {
                    // todo: apply status effects here!
                }
            }
        }
        private bool _explorationMode = true;
        public bool explorationMode
        {
            get { return _explorationMode; }
            set
            {
                _explorationMode = value;
                if (_explorationMode)
                {
                    // todo: do something?
                }
            }
        }
        public event Action PlayerDamaged; // use for other classes to know when player is damaged (shackles of pain?)

        private void OnEnable()
        {
            InitializeStats();
        }

        public void ToggleExplorationMode()
        {
            explorationMode = !explorationMode;
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

        private void UpdateGoldInfo()
        {
            goldText.text = $"Gold: {gold}";
        }

        private void UpdateXPInfo()
        {
            xpSlider.maxValue = XPToNextLevel;
            xpSlider.value = XP;
        }

        private void UpdateAPInfo()
        {
            apSlider.maxValue = maxAP;
            apSlider.value = currentAP;
            apText.text = $"{currentAP}/{maxAP}";
        }

        private void UpdateMagicArmorInfo()
        {
            magicArmorSlider.maxValue = maxMagicArmor;
            magicArmorSlider.value = currentMagicArmor;
            magicArmorText.text = $"{currentMagicArmor}/{maxMagicArmor}";
        }

        private void UpdatePhysicalArmorInfo()
        {
            physicalArmorSlider.maxValue = maxPhysicalArmor;
            physicalArmorSlider.value = currentPhysicalArmor;
            physicalArmorText.text = $"{currentPhysicalArmor}/{maxPhysicalArmor}";
        }

        private void UpdateHealthInfo()
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
            healthText.text = $"{currentHealth}/{maxHealth}";
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