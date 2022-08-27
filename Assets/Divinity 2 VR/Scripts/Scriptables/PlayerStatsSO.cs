using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "PlayerStats")]
public class PlayerStatsSO : ScriptableObject
{
    public string userName;
    public int maxHealth;
    public int currentHealth;
    public int maxPhysicalArmor;
    public int currentPhysicalArmor;
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
}
