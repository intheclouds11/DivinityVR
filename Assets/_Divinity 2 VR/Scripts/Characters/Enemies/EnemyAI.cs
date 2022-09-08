using System;
using System.Collections.Generic;
using HurricaneVR.Framework.Core.Utils;
using Pathfinding;
using UnityEngine;
using Random = UnityEngine.Random;

namespace intheclouds
{
    public class EnemyAI : MonoBehaviour
    {
        public AudioClip baseAttackHitAudioClip;
        public AudioClip baseAttackSwingAudioClip;
        public AudioClip footstepAudioClip;
        public AudioClip sheatheAudioClip;
        public AudioClip unsheatheAudioClip;
        public bool targetNearestPlayer;
        private EnemyStats enemyStats;
        private Animator animator;
        private float distanceToTarget;
        private float distanceMoved;
        private Vector3 previousPosition;
        private AIDestinationSetter aiDestinationSetter;
        private RichAI ai;
        private bool hasAttacked;
        private bool reachedTarget;
        private PlayerStats targetedPlayer;
        private SpiritWander targetedPlayerSW;
        private static readonly int _isWalking = Animator.StringToHash("isWalking");
        private static readonly int _isAttacking = Animator.StringToHash("isAttacking");
        private static readonly int _isSheathing = Animator.StringToHash("isSheathing");
        private static readonly int _isUnsheathing = Animator.StringToHash("isUnsheathing");

        private void Start()
        {
            enemyStats = GetComponent<EnemyStats>();
            animator = GetComponent<Animator>();
            aiDestinationSetter = GetComponent<AIDestinationSetter>();
            ai = GetComponent<RichAI>();
        }

        private void Update()
        {
            if (!enemyStats.Turn || !enemyStats.isAlive) return;

            if (GameManager.Instance.playersAlive == 0)
            {
                EndCombat();
                enabled = false;
                return;
            }

            if (!ai.reachedDestination && enemyStats.CurrentAP > 0)
            {
                ChaseTarget();
            }

            else if (enemyStats.CurrentAP >= 2 && !hasAttacked)
            {
                AttackTarget();
            }
            else if (enemyStats.CurrentAP < 2 && !hasAttacked)
            {
                // could retarget and move before end turn
                EndTurn();
            }
        }

        public void StartCombat()
        {
            if (enemyStats.weaponSheathed)
            {
                animator.SetBool(_isUnsheathing, true);
            }
        }

        public void StartTurn()
        {
            if (targetNearestPlayer)
            {
                Debug.Log($"targeting nearest player: {FindNearestPlayer().Name}");
                targetedPlayer = FindNearestPlayer();
                aiDestinationSetter.target = targetedPlayer.LocalUserObjects.HVRPlayerController.gameObject.transform;
            }
            else
            {
                Debug.Log($"targeting player with highest health: {FindPlayerWithHighestHealth().Name}");
                targetedPlayer = FindPlayerWithHighestHealth();
                aiDestinationSetter.target = targetedPlayer.LocalUserObjects.HVRPlayerController.gameObject.transform;
            }

            targetedPlayerSW = aiDestinationSetter.target.GetComponentInParent<LocalUserObjects>().spiritWander;
            if (targetedPlayerSW.activated)
            {
                Debug.Log("targeting spawned object");
                aiDestinationSetter.target = targetedPlayerSW.spawnedGOs[0].transform;
            }

            targetedPlayerSW.SpiritFormToggled += TargetSpiritFormToggled;
            previousPosition = transform.position;
            ai.canMove = false;
        }

        private void TargetSpiritFormToggled()
        {
            if (targetedPlayerSW.activated)
            {
                Debug.Log($"changing target to targetedPlayerSW.spawnedGOs[0].transform");
                foreach (var spawnedGO in targetedPlayerSW.spawnedGOs)
                {
                    Debug.Log($"spawnedGO: {spawnedGO}");
                }

                aiDestinationSetter.target = targetedPlayerSW.spawnedGOs[0].transform;
            }
            else
            {
                Debug.Log("targetSW not activated");
                aiDestinationSetter.target = targetedPlayer.transform;
            }
        }

        public void EndTurn()
        {
            if (enemyStats.Turn)
            {
                enemyStats.Turn = false;
            }
            else
            {
                return;
            }
            animator.SetBool(_isAttacking, false);
            animator.SetBool(_isWalking, false);
            ai.canMove = false;
            targetedPlayerSW.SpiritFormToggled -= TargetSpiritFormToggled;
        }

        public void EndCombat()
        {
            animator.SetBool(_isAttacking, false);
            animator.SetBool(_isWalking, false);
            animator.SetBool(_isUnsheathing, false);
            animator.SetBool(_isSheathing, true);
            ai.canMove = false;
        }

        private void ChaseTarget()
        {
            ai.canMove = true;
            animator.SetBool(_isWalking, true);
            TrackMovementApUsage();
        }

        private void TrackMovementApUsage()
        {
            distanceMoved += Vector3.Distance(transform.position, previousPosition);

            if (distanceMoved > 3)
            {
                enemyStats.CurrentAP -= 1;
                distanceMoved -= 3;
            }

            previousPosition = transform.position;
        }

        private void AttackTarget()
        {
            Debug.Log("enemy attacked");
            enemyStats.CurrentAP -= 2;
            ai.canMove = false;
            hasAttacked = true;
            animator.SetBool(_isWalking, false);
            animator.SetBool(_isAttacking, true);
        }

        #region Animation Events

        public void PlayBaseAttackSwingSound()
        {
            SFXPlayer.Instance.PlaySFXRandomPitchAttach(baseAttackSwingAudioClip, transform, 0.9f, 1.1f, 1f, 20);
        }

        public void PlayFootStep()
        {
            SFXPlayer.Instance.PlaySFXRandomPitchAttach(footstepAudioClip, transform, 0.95f, 1.0f, Random.Range(0.3f, 0.5f), 20);
        }

        public void DamagePlayer()
        {
            Debug.Log("DAMAGE PLAYER");
            SFXPlayer.Instance.PlaySFXRandomPitchAttach(baseAttackHitAudioClip, transform, 0.9f, 1.1f, 0.5f, 20);
            PlayerStats player;
            if (targetedPlayerSW.activated)
            {
                player = targetedPlayerSW.transform.root.GetComponent<PlayerStats>();
            }
            else
            {
                player = aiDestinationSetter.target.parent.gameObject.GetComponent<PlayerStats>();
            }

            player.TakeDamage(enemyStats.baseDamage);
            if (player.CurrentHealth == 0)
            {
                if (targetNearestPlayer)
                {
                    aiDestinationSetter.target = FindNearestPlayer().LocalUserObjects.HVRPlayerController.gameObject.transform;
                }
                else
                {
                    aiDestinationSetter.target = FindPlayerWithHighestHealth().LocalUserObjects.HVRPlayerController.gameObject.transform;
                }
            }
        }

        public void AttackAnimFinished()
        {
            hasAttacked = false;
            animator.SetBool(_isAttacking, false);
        }

        public void AttachDetachWeapon()
        {
            if (enemyStats.weaponSheathed)
            {
                SFXPlayer.Instance.PlaySFXRandomPitchAttach(unsheatheAudioClip, transform, 1f, 1f, 1f, 20);
                enemyStats.weapon.transform.SetParent(enemyStats.weaponUnsheatheParent.transform);
                enemyStats.weaponSheathed = false;
            }
            else
            {
                SFXPlayer.Instance.PlaySFXRandomPitchAttach(sheatheAudioClip, transform, 1f, 1f, 0.8f, 20);
                enemyStats.weapon.transform.SetParent(enemyStats.weaponSheatheParent.transform);
                enemyStats.weaponSheathed = true;
            }

            enemyStats.weapon.transform.localPosition = Vector3.zero;
            enemyStats.weapon.transform.localRotation = Quaternion.identity;
        }

        #endregion

        public PlayerStats FindPlayerWithHighestHealth()
        {
            int highestHealth = int.MinValue;
            PlayerStats highestHealthPlayer = null;
            foreach (PlayerStats player in GameManager.Instance.players)
            {
                if (player.CurrentHealth > highestHealth)
                {
                    highestHealth = player.CurrentHealth;
                    highestHealthPlayer = player;
                }
            }

            return highestHealthPlayer;
        }

        public PlayerStats FindNearestPlayer()
        {
            float shortestDistance = 0;
            PlayerStats nearestPlayer = null;
            foreach (PlayerStats player in GameManager.Instance.players)
            {
                var dist = Vector3.Distance(player.LocalUserObjects.HVRPlayerController.transform.position, transform.position);
                if (dist > shortestDistance)
                {
                    shortestDistance = dist;
                    nearestPlayer = player;
                }
            }

            return nearestPlayer;
        }
    }
}