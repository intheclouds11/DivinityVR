using System.Collections.Generic;
using System.Linq;
using intheclouds;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public float DistanceEnemiesJoinCombat = 15f;
    public static EnemyManager instance;
    [field: SerializeField]
    public List<EnemyStats> EnemiesInCombat { get; private set; }
    [field: SerializeField]
    public List<EnemyStats> Enemies;

    private void Awake()
    {
        instance = this;
        Enemies = FindObjectsOfType<EnemyStats>(true).ToList();
    }

    public void PopulateEnemiesInCombatList()
    {
        foreach (var playerStats in GameManager.instance.players)
        {
            foreach (var enemyStats in Enemies)
            {
                var distanceFromEnemy = Vector3.Distance(playerStats.LocalUserObjects.ITCPlayerController.transform.position, enemyStats.transform.position);
                if (distanceFromEnemy <= DistanceEnemiesJoinCombat && enemyStats.gameObject.activeSelf)
                {
                    EnemiesInCombat.Add(enemyStats);
                }
            }
        }
    }
}