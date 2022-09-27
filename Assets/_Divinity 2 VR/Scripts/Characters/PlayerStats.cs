using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using HurricaneVR.Framework.Core.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace intheclouds
{
    [SuppressMessage("ReSharper", "ArrangeAccessorOwnerBody")]
    public sealed class PlayerStats : BaseStats
    {
        public LocalUserObjects LocalUserObjects;
        private PlayerMovementAP playerMovementAP;
        [SerializeField]
        private TextMeshProUGUI goldText;
        [SerializeField]
        private AudioClip levelUpAudioClip;
        public GameObject infoPopupParent;
        public GameObject infoPopupPrefab;

        #region Player Stats

        public int CurrentHealth
        {
            get { return _currentHealth; }
            set
            {
                _currentHealth = value;
                UpdateHealthInfo();
            }
        }
        public int MaxHealth
        {
            get => _maxHealth;
            set
            {
                _maxHealth = value;
                UpdateHealthInfo();
            }
        }
        public int CurrentPoise
        {
            get { return _currentPoise; }
            set
            {
                _currentPoise = value;
                UpdatePoiseInfo();
            }
        }
        public int MaxPoise
        {
            get { return _maxPoise; }
            set
            {
                _maxPoise = value;
                UpdatePoiseInfo();
            }
        }
        public int CurrentMagicArmor
        {
            get { return _currentMagicArmor; }
            set
            {
                _currentMagicArmor = value;
                UpdateMagicArmorInfo();
            }
        }
        public int MaxMagicArmor
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
        public int Gold
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
        public bool ExplorationMode
        {
            get => _explorationMode;
            set
            {
                _explorationMode = value;
                if (_explorationMode)
                {
                    _turn = false;
                    playerMovementAP.EndTurn();
                    InCombat = false;
                }
            }
        }
        private bool _explorationMode = true;
        public override bool Turn
        {
            get { return _turn; }
            set
            {
                _turn = value;
                if (_turn)
                {
                    statusEffectsContainer.Cooldown();
                    LocalUserObjects.abilitySystem.Cooldown();
                    playerMovementAP.StartTurn();
                }
                else
                {
                    if (InCombat)
                    {
                        playerMovementAP.EndTurn();
                        GameManager.Instance.NextTurn = true;
                        RefillAP();
                    }
                }
            }
        }

        public override bool InCombat
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

        public bool PlayerControlled
        {
            get { return _playerControlled; }
            set
            {
                _playerControlled = value;
                if (_playerControlled)
                {
                    GameManager.Instance.controlledPlayer = this;
                }
            }
        }

        [SerializeField]
        private bool _playerControlled;

        #endregion

        public event Action PlayerDamaged; // use for other classes to know when player is damaged (shackles of pain?)


        private void Awake()
        {
            InitializeStats();
        }

        private void InitializeStats()
        {
            playerMovementAP = LocalUserObjects.PlayerMovementAP;
            Name = statsSO.Name;
            if (nameText)
            {
                nameText.text = Name;
            }

            MaxHealth = statsSO.maxHealth;
            CurrentHealth = MaxHealth;
            MaxPoise = statsSO.maxPoise;
            CurrentPoise = statsSO.currentPoise;
            MaxMagicArmor = statsSO.maxMagicArmor;
            CurrentMagicArmor = statsSO.currentMagicArmor;
            MaxAP = statsSO.maxAP;
            _startingAP = statsSO.startingAP;
            CurrentAP = _startingAP;

            level = statsSO.level;
            XP = statsSO.XP;
            XPToNextLevel = statsSO.XPToNextLevel;
            Gold = statsSO.gold;

            Strength = statsSO.Strength;
            Finesse = statsSO.Finesse;
            Intelligence = statsSO.Intelligence;
            Vitality = statsSO.Vitality;
            Memory = statsSO.Memory;
            Wits = statsSO.Wits;
        }

        private void UpdateGoldInfo()
        {
            goldText.text = $"Gold: {Gold}";
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

        public override void TakeDamage(BaseStats attacker, int damage, DamageType damageType, ElementalType elementalType, StatusEffect statusEffect)
        {
            this.attacker = attacker;

            // todo: reduce damage based on character resistance to elemental type

            var newHitPopup = Instantiate(hitPopupPrefab, hitPopupsParent.transform, false);
            newHitPopup.GetComponent<TextMeshProUGUI>().text = damage.ToString();

            var newInfoPopup = Instantiate(infoPopupPrefab, infoPopupParent.transform, false);
            newInfoPopup.GetComponent<TextMeshProUGUI>().text = $"{damage}";

            if (damageType == DamageType.Physical)
            {
                if (CurrentPoise - damage >= 0)
                {
                    CurrentPoise -= damage;
                    newHitPopup.GetComponent<TextMeshProUGUI>().color = Color.gray;
                    newInfoPopup.GetComponent<TextMeshProUGUI>().color = Color.gray;
                }
                else
                {
                    CurrentPoise = 0;
                    CurrentHealth -= damage - CurrentPoise;
                    statusEffectsContainer.TryAddStatusEffect(statusEffect);
                    newHitPopup.GetComponent<TextMeshProUGUI>().color = Color.red;
                    newInfoPopup.GetComponent<TextMeshProUGUI>().color = Color.red;
                }
            }
            else if (damageType == DamageType.Magic)
            {
                if (CurrentMagicArmor - damage >= 0)
                {
                    CurrentMagicArmor -= damage;
                    newHitPopup.GetComponent<TextMeshProUGUI>().color = Color.blue;
                    newInfoPopup.GetComponent<TextMeshProUGUI>().color = Color.blue;
                }
                else
                {
                    CurrentMagicArmor = 0;
                    CurrentHealth -= damage - CurrentMagicArmor;
                    statusEffectsContainer.TryAddStatusEffect(statusEffect);
                    newHitPopup.GetComponent<TextMeshProUGUI>().color = Color.red;
                    newInfoPopup.GetComponent<TextMeshProUGUI>().color = Color.red;
                }
            }

            if (CurrentHealth > 0)
            {
                SFXPlayer.Instance.PlaySFXRandomPitchAttach(hurtAudioClips[Random.Range(0, hurtAudioClips.Length - 1)],
                    LocalUserObjects.HVRPlayerController.gameObject.transform, 0.9f, 1.1f, 0.5f, 20);
            }
            else
            {
                CurrentHealth = 0;
                Debug.Log("player died!!!");
                Died();
            }

            healthSlider.value = CurrentHealth;
            PlayerDamaged?.Invoke();
        }

        public override void Heal(int healAmount, BaseStats healer = null, StatusEffect statusEffect = null)
        {
            if (_currentHealth < _maxHealth)
            {
                var prevHealth = _currentHealth;
                CurrentHealth = Math.Clamp(CurrentHealth + healAmount, 0, _maxHealth);
                Debug.Log($"Healed {Name} for {_currentHealth - prevHealth}");
            }
        }

        public void RestoreMagicArmor(int amount)
        {
            if (_currentMagicArmor < _maxMagicArmor)
            {
                var prevMA = _currentMagicArmor;
                CurrentMagicArmor = Math.Clamp(CurrentMagicArmor + amount, 0, _maxMagicArmor);
                Debug.Log($"Restored {_currentMagicArmor - prevMA} magic armor");
            }
        }
        
        public void RestorePhysicalArmor(int amount)
        {
            if (_currentPoise < _maxPoise)
            {
                var prevPA = _currentPoise;
                CurrentPoise = Math.Clamp(_currentPoise + amount, 0, _maxPoise);
                Debug.Log($"Restored {_currentPoise - prevPA} physical armor");
            }
        }

        public void Died()
        {
            SFXPlayer.Instance.PlaySFXRandomPitchAttach(deadAudioClips[Random.Range(0, deadAudioClips.Length - 1)],
                LocalUserObjects.HVRPlayerController.gameObject.transform, 0.9f, 1.1f, 1, 20);
            GameManager.Instance.playersAlive -= 1;
            GameManager.Instance.turnOrderList.Remove(new KeyValuePair<BaseStats, int>(this, Wits));
            GameManager.Instance.UpdateTurnOrderText(GameManager.Instance.turnOrderList);
        }

        public void UseAP(int apConsumed)
        {
            CurrentAP -= apConsumed;
            apSlider.value = CurrentAP;
        }

        public void ObtainXP(int xp)
        {
            Debug.Log($"get xp {Name}", this);

            var newPopup = Instantiate(infoPopupPrefab, infoPopupParent.transform, false);
            newPopup.GetComponent<TextMeshProUGUI>().text = $"+{xp} XP";

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
            SFXPlayer.Instance.PlaySFX(levelUpAudioClip, transform);
            // award 1 Attribute Point, 1 Skill Point, 1 Talent

            // todo: make Coroutine for deciding what to put points into. Make it undoable
            // example:
            // if Constitution added
            // constitution += 1;
            // playerStatsSO.maxHealth = (int) Math.Round(playerStatsSO.maxHealth * 1.07f); // adds 7% to max health
        }

        private void RefillAP()
        {
            if (_currentAP > 0)
            {
                if (_currentAP + _startingAP > _maxAP)
                {
                    CurrentAP = _maxAP;
                }
                else
                {
                    CurrentAP += _startingAP;
                }
            }
            else
            {
                CurrentAP = _startingAP;
            }
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
            statsSO.gold = Gold;
        }
    }
}