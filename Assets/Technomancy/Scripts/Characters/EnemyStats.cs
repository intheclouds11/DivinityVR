using System;
using System.Collections.Generic;
using HurricaneVR.Framework.Core.Utils;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace intheclouds
{
    public sealed class EnemyStats : BaseStats
    {
        #region Enemy Stats

        [Tooltip("Enemy Stats")]
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
                UpdatePhysicalArmorInfo();
            }
        }
        public int MaxPoise
        {
            get { return _maxPoise; }
            set
            {
                _maxPoise = value;
                UpdatePhysicalArmorInfo();
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
        public override bool Turn
        {
            get { return _turn; }
            set
            {
                _turn = value;
                if (_turn)
                {
                    statusEffectsContainer.StatusEffectCooldown();
                    enemyAI.StartTurn();
                    // todo: apply status effect damage and cooldown decrement here!
                }
                else
                {
                    enemyAI.EndTurn();
                    GameManager.Instance.NextTurn = true;
                    RefillAP();
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
                    enemyAI.StartCombat();
                }
                else
                {
                    enemyAI.EndCombat();
                }
            }
        }

        #endregion

        #region Enemy Attributes

        public int strength => statsSO.Strength;
        public int finesse => statsSO.Finesse;
        public int intelligence => statsSO.Intelligence;
        public int constitution => statsSO.Vitality;
        public int wits => statsSO.Wits;

        #endregion

        public bool weaponSheathed = true;
        public GameObject weapon;
        public GameObject weaponUnsheatheParent;
        public GameObject weaponSheatheParent;
        public bool isAlive = true;
        public event Action EnemyDamaged;
        public event Action EnemyDied;
        private EnemyAI enemyAI;
        private Animator animator;
        private static readonly int _isDead = Animator.StringToHash("isDead");
        private static readonly int _isHit = Animator.StringToHash("isHit");

        private void Awake()
        {
            enemyAI = GetComponent<EnemyAI>();
            animator = GetComponent<Animator>();
            InitializeStats();
        }

        private void InitializeStats()
        {
            Name = statsSO.Name;
            nameText.text = Name;
            MaxHealth = statsSO.maxHealth;
            CurrentHealth = MaxHealth;
            MaxPoise = statsSO.maxPoise;
            CurrentPoise = statsSO.currentPoise;
            MaxMagicArmor = statsSO.maxMagicArmor;
            CurrentMagicArmor = statsSO.currentMagicArmor;
            MaxAP = statsSO.maxAP;
            _startingAP = statsSO.startingAP;
            CurrentAP = _startingAP;
        }

        public override void TakeDamage(BaseStats attacker, int damage, DamageType damageType, ScalingType scalingType, StatusEffect statusEffect)
        {
            if (!isAlive) return;

            // Debug.Log($"hit by: {attacker}");

            this.attacker = attacker;

            if (statusEffect)
            {
                statusEffect.CombatantWhoApplied = attacker;
            }

            var newHitPopup = Instantiate(hitPopupPrefab, hitPopupsParent.transform, false);
            newHitPopup.GetComponent<TextMeshProUGUI>().text = damage.ToString();

            if (damageType == DamageType.Physical)
            {
                if (CurrentPoise - damage >= 0)
                {
                    CurrentPoise -= damage;
                    newHitPopup.GetComponent<TextMeshProUGUI>().color = Color.gray;
                }
                else
                {
                    CurrentHealth -= damage - CurrentPoise;
                    CurrentPoise = 0;
                    statusEffectsContainer.TryAddStatusEffect(statusEffect);
                    newHitPopup.GetComponent<TextMeshProUGUI>().color = Color.red;
                }
            }
            else if (damageType == DamageType.Magic)
            {
                if (CurrentMagicArmor - damage >= 0)
                {
                    CurrentMagicArmor -= damage;
                    newHitPopup.GetComponent<TextMeshProUGUI>().color = Color.blue;
                }
                else
                {
                    CurrentHealth -= damage - CurrentMagicArmor;
                    CurrentMagicArmor = 0;
                    statusEffectsContainer.TryAddStatusEffect(statusEffect);
                    newHitPopup.GetComponent<TextMeshProUGUI>().color = Color.red;
                }
            }

            if (CurrentHealth > 0)
            {
                SFXPlayer.Instance.PlaySFXRandomPitch(hurtAudioClips[Random.Range(0, hurtAudioClips.Length - 1)], transform.position, 0.9f, 1.1f, 1, 20);
                if (!weaponSheathed)
                {
                    animator.SetBool(_isHit, true);
                }

                EnemyDamaged?.Invoke();
            }

            if (!InCombat)
            {
                GameManager.Instance.UpdateGameState(GameState.CombatStart);
            }
            else if (CurrentHealth <= 0)
            {
                Died();
            }
        }

        public override void Heal(int healAmount, BaseStats healer = null, StatusEffect statusEffect = null)
        {
            var newHitPopup = Instantiate(hitPopupPrefab, hitPopupsParent.transform, false);
            newHitPopup.GetComponent<TextMeshProUGUI>().text = healAmount.ToString();
            
            if (_currentHealth < _maxHealth)
            {
                var prevHealth = _currentHealth;
                CurrentHealth = Math.Clamp(CurrentHealth + healAmount, 0, _maxHealth);
                Debug.Log($"Healed {Name} for {_currentHealth - prevHealth}");
            }
        }
        
        public override void Stun(bool stunned)
        {
            Stunned = stunned;
            
            if (Stunned)
            {
                // animator
            }
        }

        private void Died()
        {
            isAlive = false;
            CurrentHealth = 0;
            enemyAI.DisableAIComponents();
            SFXPlayer.Instance.PlaySFXRandomPitch(deadAudioClips[Random.Range(0, deadAudioClips.Length - 1)], transform.position, 0.9f, 1.1f, 0.4f, 20);
            animator.SetBool(_isDead, true);
            EnemyDied?.Invoke();
            foreach (var instancePlayer in GameManager.Instance.players)
            {
                instancePlayer.ObtainXP(statsSO.XPDefeated);
            }

            EnemyManager.Instance.enemyList.Remove(this);
            GameManager.Instance.enemiesAlive -= 1;
            GameManager.Instance.turnOrderList.Remove(new KeyValuePair<BaseStats, int>(this, wits));
            GameManager.Instance.UpdateTurnOrderText(GameManager.Instance.turnOrderList);
        }

        #region Animation Events

        public void EndHitAnimation()
        {
            animator.SetBool(_isHit, false);
        }

        public void EndDeathAnimation()
        {
            floatingStatsCanvas.SetActive(false);
        }

        #endregion

        private void UpdateMagicArmorInfo()
        {
            magicArmorSlider.maxValue = MaxMagicArmor;
            magicArmorSlider.value = CurrentMagicArmor;
            magicArmorText.text = $"{CurrentMagicArmor}/{MaxMagicArmor}";
        }

        private void UpdatePhysicalArmorInfo()
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

        private void UpdateAPInfo()
        {
            apText.text = $"{CurrentAP}/{MaxAP}";
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