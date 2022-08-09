using UnityEngine;
[CreateAssetMenu(fileName = "New EnemyStats")]
public class EnemyStatsSO : ScriptableObject
{
    public int maxHealth;
    public int currentHealth;
    public int maxAP;
    public int currentAP;
    public int XP_Earned;
}
