using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "New EnemyStats")]
public class EnemyStatsSO : ScriptableObject
{
    public int maxHealth;
    public int currentHealth;
    public int maxPhysicalArmor;
    public int currentPhysicalArmor;
    public int maxMagicArmor;
    public int currentMagicArmor;
    public int maxAP;
    public int currentAP;
    public int earnedXP;
}
