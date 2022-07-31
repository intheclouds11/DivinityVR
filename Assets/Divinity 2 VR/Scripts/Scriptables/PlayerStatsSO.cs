using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStats")]
public class PlayerStatsSO : ScriptableObject
{
    public float health;
    public float stamina;
    public float staminaRecoveryRate;
    public float staminaDepletionRateSprinting;
}
