using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace intheclouds
{
    public class RainSurfaceMaker : MonoBehaviour
    {
        public SurfaceEffect surfaceEffect;
        public int maxSpawn;
        public float hitDistance = 10;
        private int _spawnedCount;
        private WaitForSeconds _delay = new(0.5f);

        private void Start()
        {
            StartCoroutine(SpawnWaterGround());
            StartCoroutine(CheckCombatantHitRange());
        }

        private void OnDisable()
        {
            _spawnedCount = 0;
            StopAllCoroutines();
        }

        private IEnumerator CheckCombatantHitRange()
        {
            bool alreadyWet;

            foreach (PlayerStats player in GameManager.instance.players)
            {
                alreadyWet = false;
                foreach (var statusEffect in player.statusEffectsContainer.statusEffectList)
                {
                    if (statusEffect.type == StatusEffectType.Wet)
                    {
                        alreadyWet = true;
                        break;
                    }
                }

                if (!alreadyWet)
                {
                    var dist = Vector3.Distance(player.LocalUserObjects.ITCPlayerController.transform.position, transform.position);
                    if (dist < hitDistance)
                    {
                        Helpers.AddWetStatus(player, surfaceEffect.statusEffect);
                    }
                }
            }

            foreach (EnemyStats enemy in EnemyManager.instance.Enemies)
            {
                alreadyWet = false;
                foreach (var statusEffect in enemy.statusEffectsContainer.statusEffectList)
                {
                    if (statusEffect.type == StatusEffectType.Wet)
                    {
                        alreadyWet = true;
                        break;
                    }
                }

                if (!alreadyWet)
                {
                    var dist = Vector3.Distance(enemy.transform.position, transform.position);
                    if (dist < hitDistance)
                    {
                        Helpers.AddWetStatus(enemy, surfaceEffect.statusEffect);
                    }
                }
            }

            yield return _delay;
            StartCoroutine(CheckCombatantHitRange());
        }

        private IEnumerator SpawnWaterGround()
        {
            if (_spawnedCount >= maxSpawn)
            {
                yield break;
            }

            float length = 3;
            var position = transform.position;
            Vector3 randOrigin = new Vector3(position.x + Random.Range(-3, 3), position.y + Random.Range(-1, 1), position.z + Random.Range(-3, 3));
            if (Physics.Raycast(randOrigin, Vector3.down, out RaycastHit hit, length, 1 << LayerMask.NameToLayer("Ground")))
            {
                // Debug.Log($"SpawnWaterGround hit {hit.collider}");
                if (hit.transform.TryGetComponent(out SurfaceEffect preexistingEffect))
                {
                    SurfaceEffectsContainer.instance.RemoveSurfaceEffect(preexistingEffect);
                }

                Vector3 targetLocation = hit.point;
                var waterSurface = Instantiate(surfaceEffect.gameObject, targetLocation, Quaternion.identity);
                var spawnedSurface = waterSurface.GetComponent<SurfaceEffect>();
                spawnedSurface.cooldownTimer = spawnedSurface.cooldown;
                SurfaceEffectsContainer.instance.surfaceEffectsList.Add(spawnedSurface);
                _spawnedCount++;
            }
            else
            {
                // Debug.LogError("Raycast failed to find ground");
            }

            yield return _delay;
            StartCoroutine(SpawnWaterGround());
        }
    }
}