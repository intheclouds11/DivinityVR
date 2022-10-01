using System;
using HurricaneVR.Framework.Components;
using HurricaneVR.Framework.Core.Utils;
using UnityEngine;
using Random = UnityEngine.Random;

namespace intheclouds
{
    public class Fireball : AbilityBase
    {
        public float explosionRadius;
        public float explosionForce;

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
                    ResetAbilityTransform();
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

            Collider[] cols = Physics.OverlapSphere(transform.position, explosionRadius);
            foreach (var col in cols)
            {
                var rb = col.GetComponent<Rigidbody>();
                if (rb == null)
                {
                    if (col.transform.parent != null)
                    {
                        rb = col.transform.parent.GetComponent<Rigidbody>();
                    }
                }

                if (rb != null)
                {
                    rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
                }
            }

            if (!caster.LocalUserObjects.spiritWander.isActivated)
            {
                if (caster.Turn || !caster.InCombat)
                {
                    SpawnFireGround();
                    OnAbilityUsed();
                }
            }
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

                    GameObject fireSurface = Instantiate(surfaceEffect, hit.point, Quaternion.identity);
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