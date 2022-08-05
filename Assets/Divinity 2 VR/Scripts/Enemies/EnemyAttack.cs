using intheclouds;
using UnityEngine;

[RequireComponent(typeof(EnemyAI))]
public class EnemyAttack : MonoBehaviour
{
    private PlayerStats _target;
    public EnemyStatsSO enemyStatsSO;
    private float _damage;

    private void Start()
    {
        _damage = enemyStatsSO.damage;
        _target = GetComponent<EnemyAI>()._target;
    }

    public void AttackHitEvent() // this is called in the enemy animator attack event
    {
        if (_target == null)
        {
            Debug.Log("null target during attack!");
            return;
        }

        _target.TakeDamage(_damage);
    }
}