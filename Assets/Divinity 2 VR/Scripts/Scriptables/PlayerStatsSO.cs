using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "PlayerStats")]
public class PlayerStatsSO : ScriptableObject
{
    public float maxHealth;
    public float currentHealth;
    public float maxAP;
    public float currentAP;
}
