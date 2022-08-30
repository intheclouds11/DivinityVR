using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        public PlayerStats FindPlayerWithHighestHealth(List<PlayerStats> list)
        {
            if (list.Count == 0)
            {
                Debug.Log("NO PLAYERS IN GAMEMANAGER?");
            }

            int highestHealth = int.MinValue;
            PlayerStats highestHealthPlayer = null;
            foreach (PlayerStats player in list)
            {
                if (player.CurrentHealth > highestHealth)
                {
                    highestHealth = player.CurrentHealth;
                    highestHealthPlayer = player;
                }
            }

            return highestHealthPlayer;
        }

        public void StartTurn()
        {
            Debug.Log($"player with highest health: {FindPlayerWithHighestHealth(GameManager.Instance.players).Name}");
        }

        public void EndTurn()
        {
        }
    }
}