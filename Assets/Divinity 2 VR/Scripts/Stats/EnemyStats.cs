using System;
using System.Collections.Generic;
using HurricaneVR.Framework.Core.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace intheclouds
{
    public class EnemyStats : BaseStats
    {
        public int baseDamage = 15;
        public AudioClip[] hurtAudioClips;
        public AudioClip[] deadAudioClips;
        public AudioClip baseAttackWeaponAudioClip;
        public AudioClip footstepAudioClip;
        public TextMeshProUGUI apText;
        [SerializeField] private GameObject hitPopupPrefab;
        [SerializeField] private GameObject hitPopupsParent;
        public PlayerStats playerHitBy;

        #region Enemy Stats

        [Tooltip("Enemy Stats")]
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
                UpdatePhysicalArmorInfo();
            }
        }
        public override int MaxPoise
        {
            get { return _maxPoise; }
            set
            {
                _maxPoise = value;
                UpdatePhysicalArmorInfo();
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
                apText.text = $"AP: {CurrentAP}/{MaxAP}";
                if (_currentAP == 0)
                {
                    Turn = false;
                    enemyAI.EndTurn();
                }
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
                    enemyAI.StartTurn();
                    // todo: apply status effect damage and cooldown decrement here!
                }
                else
                {
                    GameManager.Instance.nextTurn = true;
                }
            }
        }

        #endregion

        #region Enemy Attributes

        public int strength => statsSO.strength;
        public int finesse => statsSO.finesse;
        public int intelligence => statsSO.intelligence;
        public int constitution => statsSO.constitution;
        public int wits => statsSO.wits;

        #endregion

        public bool isAlive = true;
        public bool attackOnSight = true;
        public bool enemyEngaged;

        public event Action EnemyDamaged;
        public event Action EnemyDied;
        private EnemyAI enemyAI;
        private List<GameObject> activeHitPopups = new List<GameObject>();


        private void Awake()
        {
            enemyAI = GetComponent<EnemyAI>();
            InitializeStats();
        }

        private void InitializeStats()
        {
            Name = statsSO.Name;
            nameText.text = Name;
            MaxHealth = statsSO.maxHealth;
            MaxPoise = statsSO.maxPoise;
            MaxMagicArmor = statsSO.maxMagicArmor;
            MaxAP = statsSO.maxAP;
            CurrentHealth = MaxHealth;
            CurrentPoise = statsSO.currentPoise;
            CurrentMagicArmor = statsSO.currentMagicArmor;
            CurrentAP = statsSO.currentAP;
            EarnedXP = statsSO.XPDefeated;
            apText.text = $"AP: {CurrentAP}/{MaxAP}";
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                if (attackOnSight && !enemyEngaged)
                {
                    GameManager.Instance.UpdateGameState(GameState.CombatStart, this);
                }
            }
        }

        public void TakeDamage(PlayerStats wieldingUser, DamageType damageType, int damage)
        {
            if (!isAlive) return;

            playerHitBy = wieldingUser;
            if (!enemyEngaged)
            {
                GameManager.Instance.UpdateGameState(GameState.CombatStart, this);
            }

            var newHitPopup = Instantiate(hitPopupPrefab, hitPopupsParent.transform, false);
            newHitPopup.GetComponent<TextMeshProUGUI>().text = damage.ToString();
            
            if (damageType == DamageType.Physical)
            {
                if (CurrentPoise - damage >= 0)
                {
                    CurrentPoise -= damage;
                }
                else
                {
                    CurrentHealth -= damage - CurrentPoise;
                    CurrentPoise = 0;
                }

                newHitPopup.GetComponent<TextMeshProUGUI>().color = Color.white;
            }
            else if (damageType == DamageType.Magic)
            {
                if (CurrentMagicArmor - damage >= 0)
                {
                    CurrentMagicArmor -= damage;
                }
                else
                {
                    CurrentHealth -= damage - CurrentMagicArmor;
                    CurrentMagicArmor = 0;
                }

                newHitPopup.GetComponent<TextMeshProUGUI>().color = Color.blue;
            }

            if (CurrentHealth > 0)
            {
                SFXPlayer.Instance.PlaySFXRandomPitchAttach(hurtAudioClips[Random.Range(0, hurtAudioClips.Length - 1)], transform, 0.9f, 1.1f, 1, 20);
                EnemyDamaged?.Invoke();
            }

            if (CurrentHealth <= 0)
            {
                Died();
            }
        }

        private void Died()
        {
            CurrentHealth = 0;
            SFXPlayer.Instance.PlaySFXRandomPitchAttach(deadAudioClips[Random.Range(0, deadAudioClips.Length - 1)], transform, 0.9f, 1.1f, 0.4f, 20);

            isAlive = false;

            EnemyDied?.Invoke();
            foreach (var instancePlayer in GameManager.Instance.players)
            {
                instancePlayer.ObtainXP(EarnedXP);
            }

            EnemyManager.Instance.enemyList.Remove(this);
            GameManager.Instance.enemiesAlive -= 1;
            GameManager.Instance.turnOrderList.Remove(new KeyValuePair<BaseStats, int>(this, wits));
            GameManager.Instance.UpdateTurnOrderText(GameManager.Instance.turnOrderList);
        }

        public void PlayBaseAttackWeaponSound() // triggered by anim event
        {
            SFXPlayer.Instance.PlaySFXRandomPitchAttach(baseAttackWeaponAudioClip, transform, 0.9f, 1.1f, 0.5f, 20);
        }

        public void PlayFootStep()
        {
            SFXPlayer.Instance.PlaySFXRandomPitchAttach(footstepAudioClip, transform, 0.95f, 1.0f, Random.Range(0.3f, 0.5f), 20);
        }

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
    }
}