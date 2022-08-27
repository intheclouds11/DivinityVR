using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace intheclouds
{
    public class CharacterAttributes : MonoBehaviour
    {
        public int strength;
        public int finesse;
        public int intelligence;
        public int constitution;
        public int wits;
        public PlayerStats playerStats;
        public EnemyStats enemyStats;
        
        void Start()
        {
            strength = playerStats ? playerStats.playerStatsSO.strength : enemyStats.enemyStatsSO.strength;
            finesse = playerStats ? playerStats.playerStatsSO.finesse : enemyStats.enemyStatsSO.finesse;
            intelligence = playerStats ? playerStats.playerStatsSO.intelligence : enemyStats.enemyStatsSO.intelligence;
            constitution = playerStats ? playerStats.playerStatsSO.constitution : enemyStats.enemyStatsSO.constitution;
            wits = playerStats ? playerStats.playerStatsSO.wits : enemyStats.enemyStatsSO.wits;
        }
        
    }
}
