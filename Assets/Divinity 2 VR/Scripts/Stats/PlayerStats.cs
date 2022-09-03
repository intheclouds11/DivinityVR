using System;
using System.Diagnostics.CodeAnalysis;
using HurricaneVR.Framework.Core.Utils;
using TMPro;
using UnityEngine;

namespace intheclouds
{
    [SuppressMessage("ReSharper", "ArrangeAccessorOwnerBody")]
    public class PlayerStats : BaseStats
    {
        public LocalUserObjects LocalUserObjects;
        private PlayerMovementAP playerMovementAP;
        [SerializeField] private TextMeshProUGUI apText;
        [SerializeField] private TextMeshProUGUI goldText;
        [SerializeField] private AudioClip levelUpClip;

        public bool explorationMode = true;

        #region Player Stats

        [Tooltip("Player Stats")]
        public override int CurrentHealth
        {
            get { return _currentHealth; }
            set
            {
                _currentHealth = value;
                UpdateHealthInfo();
            }
        }
        public override int MaxHealth
        {
            get => _maxHealth;
            set
            {
                _maxHealth = value;
                UpdateHealthInfo();
            }
        }
        public override int CurrentPoise
        {
            get { return _currentPoise; }
            set
            {
                _currentPoise = value;
                UpdatePoiseInfo();
            }
        }
        public override int MaxPoise
        {
            get { return _maxPoise; }
            set
            {
                _maxPoise = value;
                UpdatePoiseInfo();
            }
        }
        public override int CurrentMagicArmor
        {
            get { return _currentMagicArmor; }
            set
            {
                _currentMagicArmor = value;
                UpdateMagicArmorInfo();
            }
        }
        public override int MaxMagicArmor
        {
            get { return _maxMagicArmor; }
            set
            {
                _maxMagicArmor = value;
                UpdateMagicArmorInfo();
            }
        }
        public override int CurrentAP
        {
            get { return _currentAP; }
            set
            {
                _currentAP = value;
                UpdateAPInfo();
                if (_currentAP == 0)
                {
                    Turn = false;
                    playerMovementAP.EndTurn();
                }
            }
        }
        public override int MaxAP
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
        public override bool Turn
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

        public int strength => statsSO.strength;
        public int finesse => statsSO.finesse;
        public int intelligence => statsSO.intelligence;
        public int constitution => statsSO.constitution;
        public int wits => statsSO.wits;

        #endregion

        public event Action PlayerDamaged; // use for other classes to know when player is damaged (shackles of pain?)

        private void Awake()
        {
            InitializeStats();
        }

        private void InitializeStats()
        {
            playerMovementAP = GetComponent<PlayerMovementAP>();
            Name = statsSO.Name;
            MaxHealth = statsSO.maxHealth;
            MaxPoise = statsSO.maxPoise;
            MaxMagicArmor = statsSO.maxMagicArmor;
            MaxAP = statsSO.maxAP;
            CurrentHealth = MaxHealth;
            CurrentPoise = statsSO.currentPoise;
            CurrentMagicArmor = statsSO.currentMagicArmor;
            CurrentAP = statsSO.currentAP;
            XP = statsSO.XP;
            XPToNextLevel = statsSO.XPToNextLevel;
            gold = statsSO.gold;
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
            apSlider.maxValue = MaxAP;
            apSlider.value = CurrentAP;
            apText.text = $"{CurrentAP}/{MaxAP}";
        }

        private void UpdateMagicArmorInfo()
        {
            magicArmorSlider.maxValue = MaxMagicArmor;
            magicArmorSlider.value = CurrentMagicArmor;
            magicArmorText.text = $"{CurrentMagicArmor}/{MaxMagicArmor}";
        }

        private void UpdatePoiseInfo()
        {
            poiseSlider.maxValue = MaxPoise;
            poiseSlider.value = CurrentPoise;
            poiseText.text = $"{CurrentPoise}/{MaxPoise}";
        }

        private void UpdateHealthInfo()
        {
            healthSlider.maxValue = MaxHealth;
            healthSlider.value = CurrentHealth;
            healthText.text = $"{CurrentHealth}/{MaxHealth}";
        }

        public void TakeDamage(int damage)
        {
            PlayerDamaged?.Invoke();
            CurrentHealth -= damage;
            healthSlider.value = CurrentHealth;
            if (CurrentHealth <= 0)
            {
                Debug.Log("player died!!!");
                // GameManager.Instance.UpdateGameState(GameState.Lose);
            }
        }

        public void UseAP(int apConsumed)
        {
            CurrentAP -= apConsumed;
            apSlider.value = CurrentAP;
        }

        public void ObtainXP(int xp)
        {
            Debug.Log($"get xp {Name}", this);
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
            statsSO.Name = Name;

            statsSO.maxHealth = MaxHealth;
            statsSO.currentHealth = CurrentHealth;

            statsSO.maxPoise = MaxPoise;
            statsSO.currentPoise = CurrentPoise;

            statsSO.maxMagicArmor = MaxMagicArmor;
            statsSO.currentMagicArmor = CurrentMagicArmor;

            statsSO.maxAP = MaxAP;
            statsSO.currentAP = CurrentAP;
            statsSO.XP = XP;
            statsSO.gold = gold;
        }
    }
}