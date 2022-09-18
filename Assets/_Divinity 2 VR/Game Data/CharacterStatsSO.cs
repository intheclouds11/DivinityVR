using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "CharacterStats")]
public class CharacterStatsSO : ScriptableObject
{
    [Header("Stats")]
    public string Name;
    public int level;
    public int maxHealth;
    public int currentHealth;
    public int maxPoise;
    public int currentPoise;
    public int maxMagicArmor;
    public int currentMagicArmor;
    public int maxAP;
    public int currentAP;
    public int startingAP;
    public int XP;
    public int XPToNextLevel;
    public int gold;
   
    [Header("Attributes")]
    public int Strength; //+5% to all melee damage, can lift heavier objects
    public int Finesse; //+5% to all ranged physical damage
    public int Intelligence; //+5% to all int-based damage
    public int Vitality; //+7% vitality
    public int Memory; //1 extra magic slot (can't change these during combat)
    public int Wits; //+1% critical damage, +1 Initiative
   
    [Header("Skills")]
    public int Warfare; //+5% to ~all~ physical damage
    public int Huntsman; //+5% to ~all~ high ground damage (applies after other bonuses)
    public int Pyrokinetic; //+5% to all fire damage
    public int Hydrosophist; //+5% to all water damage, +5% heal amount to all heal abilities, +5% magic armour from skills and potions
    public int Aerotheurge; //+5% to all air damage
    public int Geomancer; //+5% to all earth and poison damage, +5% more physical armour from skills and potions
   
    [Header("Defense")]
    public int Retribution; //+5% damage reflected
    public int Leadership; //+2% Dodging and +3% to all resistances - Granted to all allies in a 8m radius
   
    [Header("Other")]
    public Color baseHandAugmentColor;
    
    [Header("EnemyStats")]
    public int XPDefeated;
}