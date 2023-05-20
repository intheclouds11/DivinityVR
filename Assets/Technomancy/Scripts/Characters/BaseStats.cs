using System.Collections;
using System.Collections.Generic;
using HighlightPlus;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace intheclouds
{
    // if this causes problems, just make PlayerStats and EnemyStats separate again
    public class BaseStats : MonoBehaviour
    {
        [Header("Stats")]
        public CharacterStatsSO statsSO;
        public string Name;
        public int level;
        [SerializeField]
        protected int _currentHealth;
        [SerializeField]
        protected int _maxHealth;
        [SerializeField]
        protected int _currentPoise;
        [SerializeField]
        protected int _maxPoise;
        [SerializeField]
        protected int _currentMagicArmor;
        [SerializeField]
        protected int _maxMagicArmor;
        public virtual int CurrentAP { get; set; }
        [SerializeField]
        protected int _currentAP;
        [SerializeField]
        protected int _startingAP;
        public virtual int MaxAP { get; set; }
        [SerializeField]
        protected int _maxAP;
        public virtual bool Turn { get; set; }
        [SerializeField]
        protected int credits;
        [SerializeField]
        protected bool _turn;
        [SerializeField]
        protected bool _inCombat;
        public virtual bool InCombat { get; set; }
        [field: SerializeField]
        public bool CanBackstab { get; protected set; }
        [field: SerializeField]
        public List<BaseStats> BackstabTargets { get; protected set; }
        public bool Stunned;
        public int baseDamage = 10;
        public BaseStats attacker;
        public bool pointedAtByHead;
        public bool pointedAtByHand;

        [Header("Attributes")]
        public int Strength; //+5% to all melee damage, can lift heavier objects
        public int Finesse; //+5% to all ranged physical damage
        public int Intelligence; //+5% to all int-based damage
        public int Vitality; //+7% vitality
        public int Memory; //1 extra magic slot (can't change these during combat)
        public int Wits; //+1% critical damage, +1 Initiative

        [Header("Combat Abilities")]
        [Header("Skills")]
        public int Warfare; //+5% to ~all~ physical damage
        public int Huntsman; //+5% to ~all~ high ground damage (applies after other bonuses)
        public int Pyrokinetic; //+5% to all fire damage
        public int Hydrosophist; //+5% to all water damage, +5% heal amount to all heal abilities, +5% magic armour from skills and potions
        public int Aerotheurge; //+5% to all air damage
        public int Geomancer; //+5% to all earth and poison damage, +5% more physical armour from skills and potions
        public int Scoundrel; //+5% to all dagger damage, +50% damage to backstabs
        [Header("Defense")]
        public int Retribution; //+5% damage reflected
        public int Leadership; //+2% Dodging and +3% to all resistances - Granted to all allies in a 8m radius
        // Civil Abilities: https://divinityoriginalsin2.wiki.fextralife.com/Civil+Abilities
        // Persuasion (needed for first encounter)

        [Header("Setup")]
        [SerializeField]
        protected AudioClip[] hurtAudioClips;
        [SerializeField]
        protected AudioClip[] deadAudioClips;
        [SerializeField]
        protected TextMeshProUGUI nameText;
        [SerializeField]
        protected Slider healthSlider;
        [SerializeField]
        protected Slider poiseSlider;
        [SerializeField]
        protected Slider magicArmorSlider;
        [SerializeField]
        protected Slider apSlider;
        [SerializeField]
        protected TextMeshProUGUI apText;
        [SerializeField]
        protected Slider xpSlider;
        [SerializeField]
        protected TextMeshProUGUI healthText;
        [SerializeField]
        protected TextMeshProUGUI poiseText;
        [SerializeField]
        protected TextMeshProUGUI magicArmorText;
        [SerializeField]
        protected GameObject hitPopupPrefab;
        [SerializeField]
        protected GameObject hitPopupsParent;
        [SerializeField]
        protected GameObject floatingStatsCanvas;
        [SerializeField]
        public StatusEffectsContainer statusEffectsContainer;
        public TextMeshProUGUI statusEffectsText;
        public HighlightEffect modelHighlightEffect;
        public Transform attachToCombatantTransform;

        public virtual void TakeDamage(BaseStats attacker, int damage, DamageType damageType, ScalingType scalingType, StatusEffect statusEffect)
        {
        }

        public virtual void Heal(int healAmount, BaseStats healer = null, StatusEffect statusEffect = null)
        {
        }

        public virtual void Stun(bool stunned)
        {
            
        }

        public virtual IEnumerator SkipTurn()
        {
            yield return new WaitForSeconds(1);
            GameManager.Instance.ForceNextTurn();
        }
    }
}