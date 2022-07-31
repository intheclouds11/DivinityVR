using UnityEngine;
[CreateAssetMenu(fileName = "New EnemyStats")]
public class EnemyStatsSO : ScriptableObject
{
    public float health;
    public float damage;
    public float agroRange;
    public float turningSpeed;
    public float stamina;
}
