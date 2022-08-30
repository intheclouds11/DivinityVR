using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "CharacterStats")]
public class CharacterStatsSO : ScriptableObject
{
    public string Name;
    public int maxHealth;
    public int currentHealth;
    public int maxPoise;
    public int currentPoise;
    public int maxMagicArmor;
    public int currentMagicArmor;
    public int maxAP;
    public int currentAP;
    public int XP;
    public int XPToNextLevel;
    public int gold;
    public int strength;
    public int finesse;
    public int intelligence;
    public int constitution;
    public int memory;
    public int wits;
    
    [Tooltip("EnemyStats")]
    public int XPDefeated;

}
