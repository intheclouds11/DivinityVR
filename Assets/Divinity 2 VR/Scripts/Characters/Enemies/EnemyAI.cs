using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

namespace intheclouds
{
    public class EnemyAI : MonoBehaviour
    {
        private EnemyStats enemyStats;
        private Animator animator;
        private float distanceToTarget;
        private AIDestinationSetter aiDestinationSetter;

        private void Start()
        {
            enemyStats = GetComponent<EnemyStats>();
            animator = GetComponent<Animator>();
            aiDestinationSetter = GetComponent<AIDestinationSetter>();
        }

        private void Update()
        {
            if (!enemyStats.Turn) return;
            distanceToTarget = Vector3.Distance(this.transform.position, aiDestinationSetter.target.transform.position);
            Debug.Log($"distanceToTarget: {distanceToTarget}");
        }

        public void StartTurn()
        {
            Debug.Log($"player with highest health: {FindPlayerWithHighestHealth(GameManager.Instance.players).Name}");
            aiDestinationSetter.target = FindPlayerWithHighestHealth(GameManager.Instance.players).gameObject.transform;
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

        public void EndTurn()
        {
        }
    }
}