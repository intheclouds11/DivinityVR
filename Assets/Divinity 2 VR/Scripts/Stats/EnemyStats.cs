using System;
using HurricaneVR.Framework.Core.Player;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace intheclouds
{
    public class EnemyStats : MonoBehaviour
    {
        public EnemyStatsSO enemyStatsSO;
        public int currentHealth;
        public int maxHealth;
        public int currentAP;
        public int maxAP;

        public event Action Damaged; // use for other classes to know when player is damaged

        private void Start()
        {
            maxHealth = enemyStatsSO.maxHealth;
            currentHealth = maxHealth;
            maxAP = enemyStatsSO.maxAP;
            currentAP = maxAP;
        }
        
        public void TakeDamage(float damage)
        {
            Damaged?.Invoke();
            // currentHealth -= damage;
            if (currentHealth <= 0)
            {
                // ragdoll death
            }
        }
    }
}