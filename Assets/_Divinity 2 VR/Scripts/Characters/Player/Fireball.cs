using HurricaneVR.Framework.Components;
using HurricaneVR.Framework.Core.Utils;
using UnityEngine;
using Random = UnityEngine.Random;

namespace intheclouds
{
    public class Fireball : Magic
    {
        public GameObject impactVFX;
        public AudioClip noDamageAudioClip;
        public int damage;
        public int requiredAP;
        public GameObject surfaceEffect;
        
        [HideInInspector]
        public PlayerStats wieldingUser;

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
                if (wieldingUser.LocalUserObjects.spiritWander.activated || !wieldingUser.Turn && !wieldingUser.ExplorationMode
                                                                         || wieldingUser.CurrentAP < requiredAP)
                {
                    SFXPlayer.Instance.PlaySFXRandomPitchAttach(noDamageAudioClip, wieldingUser.LocalUserObjects.Camera.transform, 1f, 1.05f, 0.7f, 20);
                    Destroy(gameObject);
                    return;
                }

                var enemy = collision.gameObject.GetComponentInParent<EnemyStats>();
                if (!enemy.isAlive)
                {
                    return;
                }

                var actualDamage = Random.Range(damage - (int) (damage * 0.1f), damage + (int) (damage * 0.1f));
                enemy.TakeDamage(wieldingUser, actualDamage, DamageType.Magic, ElementalType.Fire);

                if (!wieldingUser.ExplorationMode)
                {
                    wieldingUser.UseAP(requiredAP);
                }

                enabled = false;
            }

            if (!wieldingUser.LocalUserObjects.spiritWander.activated || wieldingUser.Turn && wieldingUser.ExplorationMode)
            {
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
            }
        }
    }
}