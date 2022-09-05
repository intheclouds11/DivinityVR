using System;
using System.Collections.Generic;
using intheclouds;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance;
    public List<EnemyStats> enemyList;

    private void Awake()
    {
        Instance = this;
    }
}