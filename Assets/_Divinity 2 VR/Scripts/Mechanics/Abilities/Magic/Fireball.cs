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
                var enemy = collision.gameObject.GetComponentInParent<EnemyStats>();
                enemy.TakeDamage(caster, Helpers.CalculateDamageRange(scaledAmount, caster), DamageType.Magic, ElementalType.Fire, statusEffect);
            }

            // EXPLOSIVE FORCE
            Collider[] cols = Physics.OverlapSphere(transform.position, explosionRadius);
            foreach (var col in cols)
            {
                var rb = col.GetComponent<Rigidbody>();
                if (rb == null)
                {
                    if (col.transform.parent)
                    {
                        rb = col.transform.parent.GetComponent<Rigidbody>();
                    }
                }

                if (rb != null)
                {
                    rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
                }
            }
            //
            
            SpawnFireGround();
            OnAbilityUsed();
            ResetAbilityTransform();
            enabled = false;
        }

        private void SpawnFireGround()
        {
            float length = 2;
            if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, length))
            {
                if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Ground") || hit.transform.gameObject.layer == LayerMask.NameToLayer("SurfaceElement"))
                {
                    // Debug.Log($"SpawnFireGround hit {hit.collider}");
                    if (hit.transform.TryGetComponent(out SurfaceEffect preexistingEffect))
                    {
                        // Debug.Log("HIT PREEXISTING FIRE SURFACE");
                        SurfaceEffectsContainer.Instance.RemoveSurfaceEffect(preexistingEffect);
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
                // Debug.LogError("Raycast failed to find ground");
            }
        }
    }
}