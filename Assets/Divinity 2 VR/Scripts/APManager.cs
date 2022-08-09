using System;
using System.Collections;
using System.Collections.Generic;
using intheclouds;
using UnityEngine;

public class APManager : MonoBehaviour
{
    public PlayerStats[] playersStats;
    public Dictionary<GameObject, PlayerStats> playersStatsDictionary;
    public EnemyStats[] enemiesStats;
    public Dictionary<GameObject, EnemyStats> enemiesStatsDictionary;
    public static APManager instance;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        playersStatsDictionary = new Dictionary<GameObject, PlayerStats>();
        enemiesStatsDictionary = new Dictionary<GameObject, EnemyStats>();
        playersStats = FindObjectsOfType<PlayerStats>();
        enemiesStats = FindObjectsOfType<EnemyStats>();
        foreach (var playerStats in playersStats)
        {
            playersStatsDictionary.Add(playerStats.gameObject, playerStats);
        }

        foreach (var enemyStats in enemiesStats)
        {
            enemiesStatsDictionary.Add(enemyStats.gameObject, enemyStats);
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
}