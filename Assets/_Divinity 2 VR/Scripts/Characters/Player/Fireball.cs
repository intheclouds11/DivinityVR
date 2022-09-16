using HurricaneVR.Framework.Components;
using HurricaneVR.Framework.Core.Utils;
using UnityEngine;
using Random = UnityEngine.Random;

namespace intheclouds
{
    public class Fireball : Magic
    {
        private void OnDisable()
        {
            var player = GameManager.Instance.FindControlledPlayer().LocalUserObjects;
            var highlight = player.handAugmentHighlight;
            highlight.overlayColor = player.PlayerStats.statsSO.baseHandAugmentColor;
            highlight.SetGlowColor(player.PlayerStats.statsSO.baseHandAugmentColor);
            highlight.highlighted = false;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Hand"))
            {
                return;
            }

            if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                if (caster.LocalUserObjects.spiritWander.activated || !caster.Turn && !caster.ExplorationMode
                                                                   || caster.CurrentAP < requiredAP)
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

                var actualDamage = Random.Range(baseDamage - (int) (baseDamage * 0.1f), baseDamage + (int) (baseDamage * 0.1f));
                enemy.TakeDamage(caster, actualDamage, DamageType.Magic, ElementalType.Fire, StatusEffect.Burning);

                enabled = false;
            }

            if (!caster.LocalUserObjects.spiritWander.activated)
            {
                if (caster.Turn)
                {
                    caster.UseAP(requiredAP);
                    var selectedMagic = magicSystem.selectedMagic.GetComponent<Magic>();
                    selectedMagic.cooldownTimer = cooldown;
                    magicSystem.DequipMagic();
                }

                SpawnFireGround();
            }

            impactVFX.transform.parent = null;
            impactVFX.SetActive(true);
            impactVFX.AddComponent<HVRDestroyTimer>().StartTimer(2);
            Destroy(gameObject);
        }

        private void SpawnFireGround()
        {
            Debug.Log("spawn fire ground");

            RaycastHit hit;
            float length = 2;
            Vector3 targetLocation;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, length))
            {
                targetLocation = hit.point;
                GameObject fireSurface = Instantiate(surfaceEffect, targetLocation, Quaternion.identity);
                fireSurface.GetComponent<SurfaceEffect>().caster = caster;
            }
        }
    }
}