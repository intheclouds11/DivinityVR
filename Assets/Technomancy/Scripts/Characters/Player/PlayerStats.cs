using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HurricaneVR.Framework.Core.Utils;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace intheclouds
{
    [SuppressMessage("ReSharper", "ArrangeAccessorOwnerBody")]
    public sealed class PlayerStats : BaseStats
    {
        public LocalUserObjects LocalUserObjects;
        private PlayerMovementAP _playerMovementAP;
        [SerializeField]
        private TextMeshProUGUI creditsText;
        [SerializeField]
        private AudioClip levelUpAudioClip;
        [field: SerializeField] public BlockIndicator BlockIndicator { get; private set; }

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
                    _playerMovementAP.EndTurn();
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
        public int Xp
        {
            get { return _XP; }
            set
            {
                _XP = value;
                UpdateXpInfo();
            }
        }
        [SerializeField]
        private int _XP;
        public int XpToNextLevel
        {
            get { return _XPToNextLevel; }
            set
            {
                _XPToNextLevel = value;
                UpdateXpInfo();
            }
        }
        [SerializeField]
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
                    _playerMovementAP.StartTurn();
                }
                else
                {
                    if (InCombat)
                    {
                        _playerMovementAP.EndTurn();
                        GameManager.instance.NextTurn = true;
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
                    _playerMovementAP.enabled = true;
                }
                else
                {
                    _playerMovementAP.enabled = false;
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
                    GameManager.instance.controlledPlayer = this;
                }
            }
        }

        [SerializeField]
        private bool _playerControlled;
        public bool Leaning
        {
            get { return _Leaning; }
            set { _Leaning = value; }
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

        private void Update()
        {
            CheckCanBackstab();
        }

        private void CheckCanBackstab()
        {
            foreach (var backstabTarget in BackstabTargets)
            {
                backstabTarget.backstabTrigger.transform.GetChild(0).gameObject.SetActive(UserInventory.instance.IsHoldingBackstabWeapon());
                CanBackstab = UserInventory.instance.IsHoldingBackstabWeapon();
            }
        }

        public void OnPlayerTriggerEnter(Collider col)
        {
            if (col.gameObject.layer == LayerMask.NameToLayer("BackstabTrigger"))
            {
                var target = col.transform.GetComponentInParent<BaseStats>() as EnemyStats;
                if (target && target.isAlive)
                {
                    BackstabTargets.Add(target);
                }
            }
        }

        public void OnPlayerTriggerExit(Collider col)
        {
            if (col.gameObject.layer == LayerMask.NameToLayer("BackstabTrigger"))
            {
                var target = col.transform.GetComponentInParent<BaseStats>() as EnemyStats;
                if (target && target.isAlive)
                {
                    BackstabTargets.Remove(target);
                    col.transform.GetChild(0).gameObject.SetActive(false);
                }
            }
        }

        private void InitializeStats()
        {
            _playerMovementAP = LocalUserObjects.PlayerMovementAP;
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
            Xp = statsSO.XP;
            XpToNextLevel = statsSO.XPToNextLevel;
            credits = statsSO.credits;

            Strength = statsSO.Strength;
            Finesse = statsSO.Finesse;
            Intelligence = statsSO.Intelligence;
            Vitality = statsSO.Vitality;
            Memory = statsSO.Memory;
            Wits = statsSO.Wits;
        }

        private void UpdateXpInfo()
        {
            xpSlider.maxValue = XpToNextLevel;
            xpSlider.value = Xp;
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

            var totalDamage = damage;

            if (BlockIndicator.inBothBlockTriggers)
            {
                totalDamage = Mathf.CeilToInt(totalDamage * 0.5f);
                BlockIndicator.GoodBlockHighlight();
                SFXPlayer.Instance.PlaySFXRandomPitch(blockAudioClips[Random.Range(0, blockAudioClips.Length - 1)],
                    LocalUserObjects.ITCPlayerController.gameObject.transform.position, 0.85f, 1, 1f);
            }

            // todo: reduce damage based on character resistance to elemental type

            var hitPopupWorld = Instantiate(hitPopupPrefab, hitPopupsParent.transform, false);
            hitPopupWorld.GetComponent<TextMeshProUGUI>().text = totalDamage.ToString();

            if (damageType == DamageType.Physical)
            {
                if (CurrentPoise - totalDamage >= 0)
                {
                    CurrentPoise -= totalDamage;
                    hitPopupWorld.GetComponent<TextMeshProUGUI>().color = Color.gray;
                    LocalUserObjects.HUDController.NewInfoPopup($"{totalDamage}", Color.gray);
                }
                else
                {
                    CurrentHealth -= totalDamage - CurrentPoise;
                    CurrentPoise = 0;
                    statusEffectsContainer.TryAddStatusEffect(statusEffect);
                    hitPopupWorld.GetComponent<TextMeshProUGUI>().color = Color.red;
                    LocalUserObjects.HUDController.NewInfoPopup($"{totalDamage}", Color.red);
                }
            }
            else if (damageType == DamageType.Magic)
            {
                if (CurrentMagicArmor - totalDamage >= 0)
                {
                    CurrentMagicArmor -= totalDamage;
                    hitPopupWorld.GetComponent<TextMeshProUGUI>().color = Color.blue;
                    LocalUserObjects.HUDController.NewInfoPopup($"{totalDamage}", Color.blue);
                }
                else
                {
                    CurrentHealth -= totalDamage - CurrentMagicArmor;
                    CurrentMagicArmor = 0;
                    statusEffectsContainer.TryAddStatusEffect(statusEffect);
                    hitPopupWorld.GetComponent<TextMeshProUGUI>().color = Color.red;
                    LocalUserObjects.HUDController.NewInfoPopup($"{totalDamage}", Color.red);
                }
            }

            if (CurrentHealth > 0)
            {
                if (BlockIndicator.inBothBlockTriggers)
                {
                    SFXPlayer.Instance.PlaySFXRandomPitch(hurtBlockedAudioClips[Random.Range(0, hurtBlockedAudioClips.Length - 1)],
                        LocalUserObjects.ITCPlayerController.gameObject.transform.position, 0.85f, 1, 0.8f);
                }
                else
                {
                    SFXPlayer.Instance.PlaySFXRandomPitch(hurtAudioClips[Random.Range(0, hurtAudioClips.Length - 1)],
                        LocalUserObjects.ITCPlayerController.gameObject.transform.position, 0.85f, 1, 0.8f);
                }
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
                var prevMa = _currentMagicArmor;
                CurrentMagicArmor = Math.Clamp(CurrentMagicArmor + amount, 0, _maxMagicArmor);
                Debug.Log($"Restored {_currentMagicArmor - prevMa} magic armor");
            }
        }

        public void RestorePhysicalArmor(int amount)
        {
            LocalUserObjects.HUDController.NewInfoPopup($"{amount}", Color.green);

            if (_currentPoise < _maxPoise)
            {
                var prevPa = _currentPoise;
                CurrentPoise = Math.Clamp(_currentPoise + amount, 0, _maxPoise);
                Debug.Log($"Restored {_currentPoise - prevPa} physical armor");
            }
        }

        public void Died()
        {
            SFXPlayer.Instance.PlaySFXRandomPitch(deadAudioClips[Random.Range(0, deadAudioClips.Length - 1)],
                LocalUserObjects.ITCPlayerController.gameObject.transform.position, 0.85f, 1, 0.8f);
            GameManager.instance.players.Remove(this);
            GameManager.instance.turnOrderList.Remove(new KeyValuePair<BaseStats, int>(this, Wits));
            GameManager.instance.UpdateTurnOrderText(GameManager.instance.turnOrderList);
        }

        public void UseAP(int apConsumed)
        {
            if (!Startup.instance.debugMode)
            {
                CurrentAP -= apConsumed;
            }
        }

        public void UpdateCredits(int amount)
        {
            var newAmount = amount + credits;
            if (newAmount > credits)
            {
                LocalUserObjects.HUDController.NewInfoPopup($"+{amount} credits", Color.yellow);
            }
            else
            {
                LocalUserObjects.HUDController.NewInfoPopup($"-{amount} credits", Color.yellow);
            }

            creditsText.text = $"Credits: {newAmount}";
            credits += amount;
        }

        public void ObtainXp(int xp)
        {
            Debug.Log($"get xp {Name}", this);

            LocalUserObjects.HUDController.NewInfoPopup($"+{xp} XP", Color.white);
            Xp += xp;

            if (Xp > XpToNextLevel)
            {
                LevelUp();
                var xpGainDelta = Xp - XpToNextLevel;

                if (xpGainDelta > XpToNextLevel)
                {
                    LevelUp();
                }
            }
        }

        public void LevelUp()
        {
            XpToNextLevel += (int) (XpToNextLevel * 0.5f);
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
    }
}