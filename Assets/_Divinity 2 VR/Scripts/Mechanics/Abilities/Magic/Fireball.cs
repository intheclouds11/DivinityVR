using System;
using HurricaneVR.Framework.Components;
using HurricaneVR.Framework.Core.Utils;
using UnityEngine;
using Random = UnityEngine.Random;

namespace intheclouds
{
    public class Fireball : AbilityBase
    {
        private void OnCollisionEnter(Collision collision)
        {
            if (!enabled)
            {
                return;
            }

            Activate(collision);
        }

        private void Activate(Collision collision)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Hand"))
            {
                return;
            }

            if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                if (caster.LocalUserObjects.spiritWander.isActivated || !caster.Turn && !caster.ExplorationMode || caster.CurrentAP < requiredAP)
                {
                    SFXPlayer.Instance.PlaySFXRandomPitchAttach(noDamageAudioClip, caster.LocalUserObjects.Camera.transform, 1f, 1.05f, 0.7f, 20);
                    Destroy(gameObject);
                    return;
                }

                var enemy = collision.gameObject.GetComponentInParent<EnemyStats>();
                if (!enemy.isAlive)
                {
                    return;
                }

                enemy.TakeDamage(caster, Helpers.CalculateDamageRange(amount, caster), DamageType.Magic, ElementalType.Fire, statusEffect);

                enabled = false;
            }

            if (!caster.LocalUserObjects.spiritWander.isActivated)
            {
                if (caster.Turn || !caster.InCombat)
                {
                    OnMagicUsed();
                    SpawnFireGround();
                }
            }

            activatedVFX.transform.parent = null;
            activatedVFX.SetActive(true);
            activatedVFX.AddComponent<HVRDestroyTimer>().StartTimer(2);
            enabled = false;
            Destroy(gameObject);
        }

        private void SpawnFireGround()
        {
            float length = 2;
            if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, length))
            {
                if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Ground") || hit.transform.gameObject.layer == LayerMask.NameToLayer("SurfaceElement"))
                {
                    Debug.Log($"SpawnFireGround hit {hit.collider}");
                    if (hit.transform.TryGetComponent(out SurfaceEffect preexistingEffect))
                    {
                        Debug.Log("HIT PREEXISTING FIRE SURFACE");
                        SurfaceEffectsContainer.Instance.RemoveSurfaceEffect(preexistingEffect);
                        // SurfaceEffectsContainer.Instance.surfaceEffectsList.Remove(preexistingEffect);
                        // Destroy(preexistingEffect.gameObject);
                    }

                    Vector3 targetLocation = hit.point;
                    GameObject fireSurface = Instantiate(surfaceEffect, targetLocation, Quaternion.identity);
                    var spawnedSurface = fireSurface.GetComponent<SurfaceEffect>();
                    spawnedSurface.caster = caster;
                    spawnedSurface.cooldownTimer = spawnedSurface.cooldown;
                    SurfaceEffectsContainer.Instance.surfaceEffectsList.Add(spawnedSurface);
                }
            }
            else
            {
                Debug.LogError("Raycast failed to find ground");
            }
        }
    }
}