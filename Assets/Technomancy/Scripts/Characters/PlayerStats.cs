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
        private TextMeshProUGUI creditsText;
        [SerializeField]
        private AudioClip levelUpAudioClip;
        

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
        private int credits;
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
                    statusEffectsContainer.StatusEffectCooldown();
                    LocalUserObjects.abilitySystem.AbilityCooldown();
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
                    _turn = false;
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
        
        public bool Leaning
        {
            get { return _Leaning; }
            set
            {
                _Leaning = value;
            }
        }

        [SerializeField]
        private bool _Leaning;

        #endregion

        public event Action PlayerDamaged; // use for other classes to know when player is damaged (shackles of pain?)

        public bool CanPerformActions(int requiredAP = 0)
        {
            return !Leaning && (!InCombat || Turn && CurrentAP >= requiredAP) && !LocalUserObjects.spiritWander.isActivated;
        }

        private void Awake()
        {
            InitializeStats();
            _playerControlled = true;
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
            credits = statsSO.gold;

            Strength = statsSO.Strength;
            Finesse = statsSO.Finesse;
            Intelligence = statsSO.Intelligence;
            Vitality = statsSO.Vitality;
            Memory = statsSO.Memory;
            Wits = statsSO.Wits;
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

        public override void TakeDamage(BaseStats attacker, int damage, DamageType damageType, ScalingType scalingType, StatusEffect statusEffect)
        {
            this.attacker = attacker;
            
            if (statusEffect)
            {
                statusEffect.CombatantWhoApplied = attacker;
            }

            // todo: reduce damage based on character resistance to elemental type

            var hitPopupWorld = Instantiate(hitPopupPrefab, hitPopupsParent.transform, false);
            hitPopupWorld.GetComponent<TextMeshProUGUI>().text = damage.ToString();

            if (damageType == DamageType.Physical)
            {
                if (CurrentPoise - damage >= 0)
                {
                    CurrentPoise -= damage;
                    hitPopupWorld.GetComponent<TextMeshProUGUI>().color = Color.gray;
                    LocalUserObjects.HUDController.NewInfoPopup($"{damage}", Color.gray);
                }
                else
                {
                    CurrentHealth -= damage - CurrentPoise;
                    CurrentPoise = 0;
                    statusEffectsContainer.TryAddStatusEffect(statusEffect);
                    hitPopupWorld.GetComponent<TextMeshProUGUI>().color = Color.red;
                    LocalUserObjects.HUDController.NewInfoPopup($"{damage}", Color.red);
                }
            }
            else if (damageType == DamageType.Magic)
            {
                if (CurrentMagicArmor - damage >= 0)
                {
                    CurrentMagicArmor -= damage;
                    hitPopupWorld.GetComponent<TextMeshProUGUI>().color = Color.blue;
                    LocalUserObjects.HUDController.NewInfoPopup($"{damage}", Color.blue);
                }
                else
                {
                    CurrentHealth -= damage - CurrentMagicArmor;
                    CurrentMagicArmor = 0;
                    statusEffectsContainer.TryAddStatusEffect(statusEffect);
                    hitPopupWorld.GetComponent<TextMeshProUGUI>().color = Color.red;
                    LocalUserObjects.HUDController.NewInfoPopup($"{damage}", Color.red);
                }
            }

            if (CurrentHealth > 0)
            {
                SFXPlayer.Instance.PlaySFXRandomPitch(hurtAudioClips[Random.Range(0, hurtAudioClips.Length - 1)],
                    LocalUserObjects.ITCPlayerController.gameObject.transform.position, 0.85f, 1, 0.8f);
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

        // add particle effect?
        public override void Heal(int healAmount, BaseStats healer = null, StatusEffect statusEffect = null)
        {
            LocalUserObjects.HUDController.NewInfoPopup($"{healAmount}", Color.red);

            if (_currentHealth < _maxHealth)
            {
                var prevHealth = _currentHealth;
                CurrentHealth = Math.Clamp(CurrentHealth + healAmount, 0, _maxHealth);
                Debug.Log($"Healed {Name} for {_currentHealth - prevHealth}");
            }
        }

        public void RestoreMagicArmor(int amount)
        {
            LocalUserObjects.HUDController.NewInfoPopup($"{amount}", Color.blue);

            if (_currentMagicArmor < _maxMagicArmor)
            {
                var prevMA = _currentMagicArmor;
                CurrentMagicArmor = Math.Clamp(CurrentMagicArmor + amount, 0, _maxMagicArmor);
                Debug.Log($"Restored {_currentMagicArmor - prevMA} magic armor");
            }
        }
        
        public void RestorePhysicalArmor(int amount)
        {
            LocalUserObjects.HUDController.NewInfoPopup($"{amount}", Color.green);

            if (_currentPoise < _maxPoise)
            {
                var prevPA = _currentPoise;
                CurrentPoise = Math.Clamp(_currentPoise + amount, 0, _maxPoise);
                Debug.Log($"Restored {_currentPoise - prevPA} physical armor");
            }
        }

        public void Died()
        {
            SFXPlayer.Instance.PlaySFXRandomPitch(deadAudioClips[Random.Range(0, deadAudioClips.Length - 1)],
                LocalUserObjects.ITCPlayerController.gameObject.transform.position, 0.85f, 1, 0.8f);
            GameManager.Instance.players.Remove(this);
            GameManager.Instance.turnOrderList.Remove(new KeyValuePair<BaseStats, int>(this, Wits));
            GameManager.Instance.UpdateTurnOrderText(GameManager.Instance.turnOrderList);
        }

        public void UseAP(int apConsumed)
        {
            if (!Startup.Instance.debugMode)
            {
                CurrentAP -= apConsumed;
            }
        }

        public void UpdateCredits(int amount)
        {
            var delta = amount - credits;
            var newAmount = amount + credits;
            if (delta > 0)
            {
                LocalUserObjects.HUDController.NewInfoPopup($"+{newAmount} credits", Color.yellow);
            }
            else
            {
                LocalUserObjects.HUDController.NewInfoPopup($"-{newAmount} credits", Color.yellow);
            }
            
            creditsText.text = $"Credits: {newAmount}";
        }

        public void ObtainXP(int xp)
        {
            Debug.Log($"get xp {Name}", this);

            LocalUserObjects.HUDController.NewInfoPopup($"+{xp} XP", Color.white);
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
            SFXPlayer.Instance.PlaySFX(levelUpAudioClip, transform.position);
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
            statsSO.gold = credits;
        }
    }
}