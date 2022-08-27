using UnityEngine;

[CreateAssetMenu(fileName = "New EnemyStats")]
public class EnemyStatsSO : ScriptableObject
{
    public string Name;
    public int maxHealth;
    public int currentHealth;
    public int maxPhysicalArmor;
    public int currentPhysicalArmor;
    public int maxMagicArmor;
    public int currentMagicArmor;
    public int maxAP;
    public int currentAP;
    public int earnedXP;
    
    public int strength;
    public int finesse;
    public int intelligence;
    public int constitution;
    public int wits;
}
