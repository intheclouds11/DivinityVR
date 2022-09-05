using System.Collections.Generic;
using HurricaneVR.Framework.Core.Utils;
using Pathfinding;
using UnityEngine;

namespace intheclouds
{
    public class EnemyAI : MonoBehaviour
    {
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
        private static readonly int _isWalking = Animator.StringToHash("isWalking");
        private static readonly int _isAttacking = Animator.StringToHash("isAttacking");

        private void Start()
        {
            enemyStats = GetComponent<EnemyStats>();
            animator = GetComponent<Animator>();
            aiDestinationSetter = GetComponent<AIDestinationSetter>();
            ai = GetComponent<RichAI>();
        }

        public void StartTurn()
        {
            if (targetNearestPlayer)
            {
                Debug.Log($"targeting nearest player: {FindNearestPlayer().Name}");
                aiDestinationSetter.target = FindNearestPlayer().LocalUserObjects.HVRPlayerController.gameObject.transform;
            }
            else
            {
                Debug.Log($"targeting player with highest health: {FindPlayerWithHighestHealth().Name}");
                aiDestinationSetter.target = FindPlayerWithHighestHealth().LocalUserObjects.HVRPlayerController.gameObject.transform;
            }

            targetedPlayer = aiDestinationSetter.target.parent.gameObject.GetComponent<PlayerStats>();


            previousPosition = transform.position;
            ai.canMove = false;
        }

        private void Update()
        {
            if (!enemyStats.Turn || !enemyStats.isAlive) return;

            if (GameManager.Instance.playersAlive == 0)
            {
                EndTurn();
                enabled = false;
                return;
            }
            
            if (enemyStats.CurrentAP <= 0)
            {
                ai.canMove = false;
                animator.SetBool(_isWalking, false);
                return;
            }

            if (!ai.reachedDestination && !reachedTarget)
            {
                ChaseTarget();
            }

            else if (enemyStats.CurrentAP >= 2 && !hasAttacked)
            {
                reachedTarget = true;
                AttackTarget();
            }
            else if (enemyStats.CurrentAP < 2 && !hasAttacked)
            {
                EndTurn();
            }
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
            Debug.Log("enemy attack");
            enemyStats.CurrentAP -= 2;
            ai.canMove = false;
            hasAttacked = true;
            Invoke(nameof(DelayNextAttack), 1.5f);
            animator.SetBool(_isWalking, false);
            animator.SetBool(_isAttacking, true);
        }

        public void DelayNextAttack()
        {
            hasAttacked = false;
            animator.SetBool(_isAttacking, false);
        }

        public void DamagePlayer() // used by animation event
        {
            Debug.Log("DAMAGE PLAYER");
            var player = aiDestinationSetter.target.parent.gameObject.GetComponent<PlayerStats>();
            player.TakeDamage(enemyStats.baseDamage);
            if (player.CurrentHealth == 0)
            {
                if (targetNearestPlayer)
                {
                    FindNearestPlayer();
                }
                else
                {
                    FindPlayerWithHighestHealth();
                }
            }
        }

        public void EndTurn()
        {
            animator.SetBool(_isAttacking, false);
            animator.SetBool(_isWalking, false);
            reachedTarget = false;
            ai.canMove = false;
            enemyStats.Turn = false;
            targetedPlayer = null;
        }

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
                var dist = Vector3.Distance(player.gameObject.transform.position, transform.position);
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