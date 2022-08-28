using System;
using System.Diagnostics.CodeAnalysis;
using HurricaneVR.Framework.Core.Player;
using HurricaneVR.Framework.Core.Utils;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace intheclouds
{
    [SuppressMessage("ReSharper", "ArrangeAccessorOwnerBody")]
    public class PlayerStats : MonoBehaviour, ICharacter
    {
        public PlayerStatsSO playerStatsSO;
        private PlayerMovementAP playerMovementAP;
        public string Name { get; set; }
        public GameObject CharacterType { get; set; }
        [SerializeField] private Slider healthSlider;
        [SerializeField] private Slider physicalArmorSlider;
        [SerializeField] private Slider magicArmorSlider;
        [SerializeField] private Slider apSlider;
        [SerializeField] private Slider xpSlider;
        [SerializeField] private TextMeshProUGUI healthText;
        [SerializeField] private TextMeshProUGUI physicalArmorText;
        [SerializeField] private TextMeshProUGUI magicArmorText;
        [SerializeField] private TextMeshProUGUI apText;
        [SerializeField] private TextMeshProUGUI goldText;
        [SerializeField] private AudioClip levelUpClip;

        public bool explorationMode = true;

        #region Player Stats

        [Tooltip("Player Stats")]
        public int currentHealth
        {
            get { return _currentHealth; }
            set
            {
                _currentHealth = value;
                UpdateHealthInfo();
            }
        }
        private int _currentHealth;
        public int maxHealth
        {
            get => _maxHealth;
            set
            {
                _maxHealth = value;
                UpdateHealthInfo();
            }
        }
        private int _maxHealth;
        public int currentPhysicalArmor
        {
            get { return _currentPhysicalArmor; }
            set
            {
                _currentPhysicalArmor = value;
                UpdatePhysicalArmorInfo();
            }
        }
        private int _currentPhysicalArmor;
        public int maxPhysicalArmor
        {
            get { return _maxPhysicalArmor; }
            set
            {
                _maxPhysicalArmor = value;
                UpdatePhysicalArmorInfo();
            }
        }
        private int _maxPhysicalArmor;
        public int currentMagicArmor
        {
            get { return _currentMagicArmor; }
            set
            {
                _currentMagicArmor = value;
                UpdateMagicArmorInfo();
            }
        }
        private int _currentMagicArmor;
        public int maxMagicArmor
        {
            get { return _maxMagicArmor; }
            set
            {
                _maxMagicArmor = value;
                UpdateMagicArmorInfo();
            }
        }
        private int _maxMagicArmor;
        public int currentAP
        {
            get { return _currentAP; }
            set
            {
                _currentAP = value;
                UpdateAPInfo();
                if (_currentAP == 0)
                {
                    turn = false;
                    playerMovementAP.EndTurn();
                }
            }
        }
        private int _currentAP;
        public int maxAP
        {
            get { return _maxAP; }
            set
            {
                _maxAP = value;
                UpdateAPInfo();
            }
        }
        private int _maxAP;
        public int gold
        {
            get { return _gold; }
            set
            {
                _gold = value;
                UpdateGoldInfo();
            }
        }
        private int _gold;
        public int XP
        {
            get { return _XP; }
            set
            {
                _XP = value;
                UpdateXPInfo();
            }
        }
        private int _XP;
        public int XPToNextLevel
        {
            get { return _XPToNextLevel; }
            set
            {
                _XPToNextLevel = value;
                UpdateXPInfo();
            }
        }
        private int _XPToNextLevel;
        public bool turn
        {
            get { return _turn; }
            set
            {
                _turn = value;
                if (_turn)
                {
                    playerMovementAP.StartTurn();
                    // todo: apply status effects here!
                }
                else
                {
                    GameManager.Instance.nextTurn = true;
                }
            }
        }
        private bool _turn;
        public bool inCombat
        {
            get { return _inCombat; }
            set
            {
                _inCombat = value;
                if (_inCombat)
                {
                    playerMovementAP.enabled = true;
                }
                else
                {
                    playerMovementAP.enabled = false;
                }
            }
        }
        private bool _inCombat;
        public bool playerControlled
        {
            get { return _playerControlled; }
            set
            {
                _playerControlled = value;
            }
        }
        [SerializeField]
        private bool _playerControlled;

        #endregion

        #region Player Attributes

        public int strength => playerStatsSO.strength;
        public int finesse => playerStatsSO.finesse;
        public int intelligence => playerStatsSO.intelligence;
        public int constitution => playerStatsSO.constitution;
        public int wits => playerStatsSO.wits;

        #endregion

        public event Action PlayerDamaged; // use for other classes to know when player is damaged (shackles of pain?)

        private void Awake()
        {
            InitializeStats();
        }

        private void InitializeStats()
        {
            playerMovementAP = GetComponent<PlayerMovementAP>();
            CharacterType = gameObject;
            Name = playerStatsSO.userName;
            maxHealth = playerStatsSO.maxHealth;
            maxPhysicalArmor = playerStatsSO.maxPhysicalArmor;
            maxMagicArmor = playerStatsSO.maxMagicArmor;
            maxAP = playerStatsSO.maxAP;
            currentHealth = maxHealth;
            currentPhysicalArmor = playerStatsSO.currentPhysicalArmor;
            currentMagicArmor = playerStatsSO.currentMagicArmor;
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

            // todo: make Coroutine for deciding what to put points into. Make it undoable
            // example:
            // if Constitution added
            // constitution += 1;
            // playerStatsSO.maxHealth = (int) Math.Round(playerStatsSO.maxHealth * 1.07f); // adds 7% to max health
        }

        public void SaveProgress()
        {
            playerStatsSO.userName = Name;

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