using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace intheclouds
{
    public class EnemyAI : MonoBehaviour
    {
        private EnemyStats enemyStats;
        
        private void Start()
        {
            enemyStats = GetComponent<EnemyStats>();
        }

        public void StartTurn()
        {
            
        }

        public void EndTurn()
        {
            
        }
    }
}
