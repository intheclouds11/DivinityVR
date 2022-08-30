using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace intheclouds
{
    // if this causes problems, just make PlayerStats and EnemyStats separate again
    public class BaseStats : MonoBehaviour
    {
        public CharacterStatsSO statsSO;
        public string Name { get; protected set; }
        [SerializeField] protected Slider healthSlider;
        [SerializeField] protected Slider poiseSlider;
        [SerializeField] protected Slider magicArmorSlider;
        [SerializeField] protected Slider apSlider;
        [SerializeField] protected Slider xpSlider;
        [SerializeField] protected TextMeshProUGUI healthText;
        [SerializeField] protected TextMeshProUGUI poiseText;
        [SerializeField] protected TextMeshProUGUI magicArmorText;

        public virtual int CurrentHealth { get; set; }
        protected int _currentHealth;
        public virtual int MaxHealth { get; set; }
        protected int _maxHealth;
        public virtual int CurrentPoise { get; set; }
        protected int _currentPoise;
        public virtual int MaxPoise { get; set; }
        protected int _maxPoise;
        public virtual int CurrentMagicArmor { get; set; }
        protected int _currentMagicArmor;
        public virtual int MaxMagicArmor { get; set; }
        protected int _maxMagicArmor;
        public virtual int CurrentAP { get; set; }
        protected int _currentAP;
        public virtual int MaxAP { get; set; }
        protected int EarnedXP { get; set; }
        public virtual bool Turn { get; set; }
        protected bool _turn;
    }
}