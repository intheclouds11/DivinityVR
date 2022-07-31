using intheclouds;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public EnemyStatsSO enemyStatsSO;
    [SerializeField] private float _agroRange;
    private float _turningSpeed;

    private NavMeshAgent _navMeshAgent;
    private float _distanceToTarget = Mathf.Infinity;
    private bool _isProvoked;

    private Animator _animator;
    private static readonly int AttackAnimation = Animator.StringToHash("attack");
    private static readonly int IdleAnimation = Animator.StringToHash("idle");
    private static readonly int MoveAnimation = Animator.StringToHash("move");

    internal PlayerStats _target;
    private EnemyHealth health;

    public void PlayMoan()
    {
        var audioSource = GetComponent<AudioSource>();
        if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

    private void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        health = GetComponent<EnemyHealth>();
        _target = FindObjectOfType<PlayerStats>();
    }

    private void Start()
    {
        _turningSpeed = enemyStatsSO.turningSpeed;
    }

    private void OnEnable()
    {
        health.Damaged += OnDamageTaken;
    }

    private void OnDisable()
    {
        health.Damaged -= OnDamageTaken;
    }

    private void Update()
    {
        if (health.IsDead())
        {
            _navMeshAgent.enabled = false;
            enabled = false;
            return;
        }

        _distanceToTarget = Vector3.Distance(_target.transform.position, this.transform.position);

        if (_isProvoked)
        {
            EngageTarget();
        }
        else if (_distanceToTarget <= _agroRange)
        {
            _isProvoked = true;
        }
    }

    private void OnDamageTaken()
    {
        _isProvoked = true;
    }

    private void EngageTarget()
    {
        if (_target.currentHealth <= 0)
        {
            _animator.SetBool(AttackAnimation, false);
            _animator.SetTrigger(IdleAnimation);
            _navMeshAgent.SetDestination(transform.position);
            return;
        }

        FaceTarget();
        if (_distanceToTarget >= _navMeshAgent.stoppingDistance)
        {
            ChaseTarget();
        }

        if (_distanceToTarget <= _navMeshAgent.stoppingDistance)
        {
            AttackTarget();
        }
    }

    private void ChaseTarget()
    {
        PlayMoan();

        if (_target.currentHealth < 0) return;
        _navMeshAgent.SetDestination(_target.transform.position);
        _animator.SetTrigger(MoveAnimation);
        _animator.SetBool(AttackAnimation, false); // stop attack animation when chasing
    }

    private void AttackTarget()
    {
        _animator.SetBool(AttackAnimation, true); // attack animation invokes AttackHitEvent()
    }

    private void FaceTarget()
    {
        // want to make transform.rotation = target transform.rotation, rotate at a certain speed
        // direction == position 1 vector - position 2 vector, normalized (magnitude 1)

        Vector3 direction = (_target.transform.position - this.transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, _turningSpeed * Time.deltaTime);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _agroRange);
    }
}