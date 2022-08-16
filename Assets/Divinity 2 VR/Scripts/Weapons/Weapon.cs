using HurricaneVR.Framework.Core;
using intheclouds;
using UnityEngine;
using UnityEngine.Serialization;

namespace intheclouds
{
    public class Weapon : MonoBehaviour
    {
        private PlayerStats wieldingUser;
        public float requiredHitSpeed = 1f;
        public int requiredHitAP = 2;
        public int physicalDamage = 10;
        public int magicDamage = 0;
        private Rigidbody rb;
        private HVRGrabbable grabbable;
        public float hitCooldownTimer;
        public float hitCooldown;

        public enum DamageType
        {
            Physical,
            Magic
        }

        private void Awake()
        {
            grabbable = GetComponent<HVRGrabbable>();
            rb = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            if (wieldingUser == null || !wieldingUser.playerTurnCombat || wieldingUser.currentAP < requiredHitAP) return;

            if (hitCooldownTimer > 0)
            {
                hitCooldownTimer -= Time.deltaTime;
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (hitCooldownTimer > 0 || wieldingUser == null || !wieldingUser.playerTurnCombat) return;
            if (collision.gameObject.CompareTag("Enemy"))
            {
                if (rb.velocity.magnitude > requiredHitSpeed)
                {
                    if (wieldingUser.currentAP > requiredHitAP)
                    {
                        collision.gameObject.GetComponent<EnemyStats>()?.TakeDamage(DamageType.Physical, physicalDamage);
                        wieldingUser.UseAP(requiredHitAP);
                        hitCooldownTimer += hitCooldown;
                    }
                }
            }
        }

        public void UpdateWielder()
        {
            if (grabbable.PrimaryGrabber == null)
            {
                wieldingUser = null;
                Debug.Log("Weapon dropped! wieldingUser == null");
            }
            else
            {
                wieldingUser = grabbable.PrimaryGrabber.transform.root.GetComponentInChildren<PlayerStats>();
                Debug.Log($"Weapon grabbed! wieldingUser: {wieldingUser.userName}");
            }
        }
    }
}