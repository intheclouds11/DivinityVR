using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace intheclouds
{
    public class RainSurfaceMaker : MonoBehaviour
    {
        public SurfaceEffect surfaceEffect;
        public int maxSpawn;
        public float hitDistance = 15;
        private int spawnedCount;

        private void Start()
        {
            InvokeRepeating(nameof(SpawnWaterGround), 0.5f, 0.5f);
            InvokeRepeating(nameof(CheckCombatantHitRange), 0.5f, 0.5f);
        }

        private void OnDisable()
        {
            spawnedCount = 0;
            CancelInvoke();
        }
        
        private void CheckCombatantHitRange()
        {
            foreach (PlayerStats player in GameManager.Instance.players)
            {
                foreach (var statusEffect in player.statusEffectsContainer.statusEffectList)
                {
                    if (statusEffect.type == StatusEffect.StatusEffectType.Wet)
                    {
                        return;
                    }
                }
                var dist = Vector3.Distance(player.LocalUserObjects.HVRPlayerController.transform.position, transform.position);
                if (dist < hitDistance)
                {
                    Helpers.MakePlayerWet(player, surfaceEffect.statusEffect);
                }
            }

            foreach (EnemyStats enemy in EnemyManager.Instance.enemyList)
            {
                foreach (var statusEffect in enemy.statusEffectsContainer.statusEffectList)
                {
                    if (statusEffect.type == StatusEffect.StatusEffectType.Wet)
                    {
                        return;
                    }
                }
                var dist = Vector3.Distance(enemy.transform.position, transform.position);
                if (dist < hitDistance)
                {
                    Helpers.MakeEnemyWet(enemy, surfaceEffect.statusEffect);
                }
            }
        }
        
        private void SpawnWaterGround()
        {
            if (!enabled)
            {
                return;
            }

            if (spawnedCount >= maxSpawn)
            {
                enabled = false;
            }

            float length = 3;
            var position = transform.position;
            Vector3 randOrigin = new Vector3(position.x + Random.Range(-3, 3), position.y + Random.Range(-1, 1), position.z + Random.Range(-3, 3));
            if (Physics.Raycast(randOrigin, Vector3.down, out RaycastHit hit, length, 1 << LayerMask.NameToLayer("Ground")))
            {
                // Debug.Log($"SpawnWaterGround hit {hit.collider}");
                if (hit.transform.TryGetComponent(out SurfaceEffect preexistingEffect))
                {
                    SurfaceEffectsContainer.Instance.RemoveSurfaceEffect(preexistingEffect);
                }

                Vector3 targetLocation = hit.point;
                var waterSurface = Instantiate(surfaceEffect.gameObject, targetLocation, Quaternion.identity);
                var spawnedSurface = waterSurface.GetComponent<SurfaceEffect>();
                spawnedSurface.cooldownTimer = spawnedSurface.cooldown;
                SurfaceEffectsContainer.Instance.surfaceEffectsList.Add(spawnedSurface);
                spawnedCount++;
            }
            else
            {
                // Debug.LogError("Raycast failed to find ground");
            }
        }
    }
}