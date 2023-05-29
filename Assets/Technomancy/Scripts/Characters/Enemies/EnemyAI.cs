using System.Linq;
using HurricaneVR.Framework.Core.Utils;
using Pathfinding;
using Pathfinding.RVO;
using UnityEngine;
using Random = UnityEngine.Random;

namespace intheclouds
{
    public class EnemyAI : MonoBehaviour
    {
        public bool attackOnSight = true;
        public bool targetNearestPlayer;
        [Header("Setup")]
        public AudioClip baseAttackHitAudioClip;
        public AudioClip baseAttackSwingAudioClip;
        public AudioClip footstepAudioClip;
        public AudioClip sheatheAudioClip;
        public AudioClip unsheatheAudioClip;
        
        private EnemyStats _enemyStats;
        private AIDestinationSetter _aiDestinationSetter;
        private RichAI _ai;
        private Animator _animator;
        private float _distanceToTarget;
        private float _distanceMoved;
        private Vector3 _previousPosition;
        private bool _hasAttacked;
        private bool _reachedTarget;
        private PlayerStats _targetedPlayer;
        private SpiritWander _targetedPlayerSW;
        private bool _wasMovingBeforeHit;
        private static readonly int IsWalking = Animator.StringToHash("isWalking");
        private static readonly int IsAttacking = Animator.StringToHash("isAttacking");
        private static readonly int IsSheathing = Animator.StringToHash("isSheathing");
        private static readonly int IsUnsheathing = Animator.StringToHash("isUnsheathing");
        private static readonly int IsDead = Animator.StringToHash("isDead");
        private static readonly int IsHit = Animator.StringToHash("isHit");


        private void Awake()
        {
            _enemyStats = GetComponent<EnemyStats>();
            _animator = GetComponent<Animator>();
            _aiDestinationSetter = GetComponent<AIDestinationSetter>();
            _ai = GetComponent<RichAI>();
        }

        private void Start()
        {
            _enemyStats.EnemyDamaged += OnEnemyDamaged;
            _enemyStats.EnemyDied += OnEnemyDied;
        }

        private void OnEnemyDamaged()
        {
            _wasMovingBeforeHit = _ai.canMove;
            _ai.canMove = false;
            // animator.SetBool(_isAttacking, false);
            // animator.SetBool(_isWalking, false);
            _animator.SetBool(IsUnsheathing, false);
            _animator.SetBool(IsSheathing, false);
            _animator.SetBool(IsHit, true);
        }

        private void OnEnemyDied()
        {
            _animator.SetBool(IsAttacking, false);
            _animator.SetBool(IsWalking, false);
            _animator.SetBool(IsUnsheathing, false);
            _animator.SetBool(IsSheathing, false);
            _animator.SetBool(IsHit, false);
            _animator.SetBool(IsDead, true);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                if (!_enemyStats.InCombat && attackOnSight)
                {
                    if (!GameManager.instance.activeCombatant)
                    {
                        GameManager.instance.UpdateGameState(GameState.CombatStart);
                    }
                    else
                    {
                        GameManager.instance.EnemyJoinedCombat(_enemyStats);
                    }
                }
            }
        }

        private void Update()
        {
            if (!_enemyStats.Turn || !_enemyStats.isAlive) return;

            if (!GameManager.instance.players.Any())
            {
                EndCombat();
                enabled = false;
                return;
            }

            // todo: add other attacks based on what skills enemy has

            if (!_animator.GetBool(IsHit))
            {
                BaseAttack();
            }
        }

        private void BaseAttack()
        {
            if (!_ai.reachedDestination && _enemyStats.CurrentAP > 0)
            {
                ChaseTarget();
            }

            else if (_enemyStats.CurrentAP >= 2 && !_hasAttacked)
            {
                AttackTarget();
            }
            else if (_enemyStats.CurrentAP < 2 && !_hasAttacked)
            {
                GameManager.instance.ForceNextTurn();
            }

            if (_ai.reachedDestination)
            {
                _ai.canMove = false;
            }
        }

        public void StartCombat()
        {
            if (_enemyStats.weaponSheathed)
            {
                _animator.SetBool(IsUnsheathing, true);
            }
        }

        public void StartTurn()
        {
            if (_enemyStats.Stunned)
            {
                enabled = false;
                StartCoroutine(_enemyStats.SkipTurn());
                return;
            }
            else
            {
                enabled = true;
            }
            if (targetNearestPlayer)
            {
                _targetedPlayer = FindNearestPlayer();
                _aiDestinationSetter.target = _targetedPlayer.LocalUserObjects.ITCPlayerController.transform;
            }
            else
            {
                _targetedPlayer = FindPlayerWithHighestHealth();
                _aiDestinationSetter.target = _targetedPlayer.LocalUserObjects.ITCPlayerController.transform;
            }

            _targetedPlayerSW = _aiDestinationSetter.target.GetComponentInParent<LocalUserObjects>().spiritWander;
            if (_targetedPlayerSW.isActivated)
            {
                _aiDestinationSetter.target = _targetedPlayerSW.spawnedGOs[0].transform;
            }

            _targetedPlayerSW.SpiritFormToggled += TargetSpiritFormToggled;
            _previousPosition = transform.position;
            _ai.canMove = false;
        }

        private void TargetSpiritFormToggled()
        {
            if (_targetedPlayerSW.isActivated)
            {
                _aiDestinationSetter.target = _targetedPlayerSW.spawnedGOs[0].transform;
            }
            else
            {
                _aiDestinationSetter.target = _targetedPlayer.LocalUserObjects.ITCPlayerController.transform;
            }
        }

        public void EndTurn()
        {
            AttackAnimFinished();
            _animator.SetBool(IsWalking, false);
            _ai.canMove = false;
            if (_targetedPlayerSW)
            {
                _targetedPlayerSW.SpiritFormToggled -= TargetSpiritFormToggled;
            }
        }

        public void EndCombat()
        {
            _animator.SetBool(IsAttacking, false);
            _animator.SetBool(IsWalking, false);
            _animator.SetBool(IsUnsheathing, false);
            _animator.SetBool(IsSheathing, true);
            _ai.canMove = false;
        }

        private void ChaseTarget()
        {
            _ai.canMove = true;
            _animator.SetBool(IsWalking, true);
            TrackMovementApUsage();
        }

        private void TrackMovementApUsage()
        {
            _distanceMoved += Vector3.Distance(transform.position, _previousPosition);

            if (_distanceMoved > 3)
            {
                _enemyStats.CurrentAP -= 1;
                _distanceMoved -= 3;
            }

            _previousPosition = transform.position;
        }

        private void AttackTarget()
        {
            _enemyStats.CurrentAP -= 2;
            _ai.canMove = false;
            _hasAttacked = true;
            _animator.SetBool(IsWalking, false);
            _animator.SetBool(IsAttacking, true);
        }

        public PlayerStats FindPlayerWithHighestHealth()
        {
            int highestHealth = int.MinValue;
            PlayerStats highestHealthPlayer = null;
            foreach (PlayerStats player in GameManager.instance.players)
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
            foreach (PlayerStats player in GameManager.instance.players)
            {
                var dist = Vector3.Distance(player.LocalUserObjects.ITCPlayerController.transform.position, transform.position);
                if (dist > shortestDistance)
                {
                    shortestDistance = dist;
                    nearestPlayer = player;
                }
            }

            return nearestPlayer;
        }

        public void DisableAIComponents()
        {
            _ai.enabled = false;
            _aiDestinationSetter.enabled = false;
            GetComponent<RVOController>().enabled = false;
            GetComponent<Seeker>().enabled = false;
        }

        #region Animation Events

        public void EndUnsheathingAnimation()
        {
            _animator.SetBool(IsUnsheathing, false);
        }

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
            SFXPlayer.Instance.PlaySFXRandomPitchAttach(baseAttackHitAudioClip, transform, 0.9f, 1.1f, 0.5f, 20);
            PlayerStats player;
            if (_targetedPlayerSW.isActivated)
            {
                player = _targetedPlayerSW.transform.GetComponentInParent<PlayerStats>();
            }
            else
            {
                player = _targetedPlayer;
            }

            player.TakeDamage(_enemyStats, _enemyStats.baseDamage, DamageType.Physical, ScalingType.None, null);
            if (player.CurrentHealth == 0)
            {
                if (targetNearestPlayer)
                {
                    _aiDestinationSetter.target = FindNearestPlayer()?.LocalUserObjects.waist.transform;
                }
                else
                {
                    _aiDestinationSetter.target = FindPlayerWithHighestHealth()?.LocalUserObjects.waist.transform;
                }
            }
        }

        public void AttackAnimFinished()
        {
            _hasAttacked = false;
            _animator.SetBool(IsAttacking, false);
        }
        
        public void EndHitAnimation()
        {
            _animator.SetBool(IsHit, false);
            if (_wasMovingBeforeHit)
            {
                _wasMovingBeforeHit = false;
                _ai.canMove = true;
            }
        }

        public void AttachDetachWeapon()
        {
            if (_enemyStats.weaponSheathed)
            {
                SFXPlayer.Instance.PlaySFXRandomPitchAttach(unsheatheAudioClip, transform, 1f, 1f, 1f, 20);
                _enemyStats.weapon.transform.SetParent(_enemyStats.weaponUnsheatheParent.transform);
                _enemyStats.weaponSheathed = false;
            }
            else
            {
                SFXPlayer.Instance.PlaySFXRandomPitchAttach(sheatheAudioClip, transform, 1f, 1f, 0.8f, 20);
                _enemyStats.weapon.transform.SetParent(_enemyStats.weaponSheatheParent.transform);
                _enemyStats.weaponSheathed = true;
            }

            _enemyStats.weapon.transform.localPosition = Vector3.zero;
            _enemyStats.weapon.transform.localRotation = Quaternion.identity;
        }

        #endregion
    }
}