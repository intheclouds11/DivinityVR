using System;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

/// <summary>
/// A static class for general helpful methods
/// </summary>
namespace intheclouds
{
    public static class Helpers
    {
        public static int CalculateDamageRange(int inputDamage, BaseStats attacker, float criticalMultiplier = 1)
        {
            var low = (int) Math.Floor(inputDamage * 0.15f);
            if (low == 0)
            {
                low = 1;
            }

            var high = (int) Math.Ceiling(inputDamage * 0.15f);
            if (high == 0)
            {
                high = 1;
            }

            Debug.Log($"low = {low}");
            Debug.Log($"high = {high}");

            if (criticalMultiplier > 1)
            {
                int critChanceRange = new System.Random().Next(1, 101);
                if (critChanceRange <= CalculateCriticalChance(attacker))
                {
                    Debug.Log("CRITICAL HIT");
                    return (int) Math.Ceiling(Random.Range(inputDamage - low, inputDamage + high) * criticalMultiplier);
                }
            }
            
            return Random.Range(inputDamage - low, inputDamage + high);
        }

        public static float CalculateCriticalChance(BaseStats combatant)
        {
            return combatant.Wits - 10;
        }
    }
}