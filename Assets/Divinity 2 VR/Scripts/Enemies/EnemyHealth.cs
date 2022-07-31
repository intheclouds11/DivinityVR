using System;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public EnemyStatsSO enemyStatsSO;
    [SerializeField] private float currentHealth;
    private bool _isDead;
    private static readonly int DieAnimation = Animator.StringToHash("die");

    public event Action Damaged;

    private void Start()
    {
        currentHealth = enemyStatsSO.health;
    }

    public bool IsDead()
    {
        return _isDead;
    }

    public void TakeDamage(float damage)
    {
        Damaged?.Invoke();
        currentHealth -= damage;
        Debug.Log($"Hit! HP remaining: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();

            EnemyManager.Instance._currentEnemyList.Remove(this.gameObject);
            if (EnemyManager.Instance._currentEnemyList.Count <= 0)
            {
                GameManager.Instance.UpdateGameState(GameState.Win);
            }
            // Destroy(this.gameObject);
        }
    }

    private void Die()
    {
        if (_isDead) return;
        GetComponent<Animator>().SetTrigger(DieAnimation);
        _isDead = true;
    }
}