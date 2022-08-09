using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "PlayerStats")]
public class PlayerStatsSO : ScriptableObject
{
    public int maxHealth;
    public int currentHealth;
    public int maxAP;
    public int currentAP;
    public int XP;
    public int gold;
}
