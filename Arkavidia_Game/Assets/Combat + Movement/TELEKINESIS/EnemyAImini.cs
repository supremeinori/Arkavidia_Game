using UnityEngine;
using UnityEngine.AI;

public class EnemyAIMini : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Animator animator;
    public PlayerHealth playerHealth;

    [Header("Mini Boss UI")]
    public BossHealthMini bossHealthMini;   // ✅ FIX DI SINI

    [Header("Damage")]
    public int attackDamage = 10;

    [Header("Settings")]
    public float detectionRadius = 15f;
    public float attackRange = 2f;
    public float patrolRadius = 20f;
    public float attackCooldown = 2f;
    public float patrolIdleTime = 3f;
    public float rotationSpeed = 7f;
    public float attackDuration = 1.0f;

    NavMeshAgent agent;
    float cooldownTimer;
    float idleTimer;
    float attackTimer;

    Vector3 patrolPoint;
    bool isPatrolling;
    bool isIdle;
    bool isAttacking;

    enum State { Patrol, Chase, Attack }
    State currentState;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (animator == null) animator = GetComponent<Animator>();
        if (playerHealth == null && player != null)
            playerHealth = player.GetComponent<PlayerHealth>();

        SetNewPatrolPoint();
        currentState = State.Patrol;
    }

    void Update()
    {
        cooldownTimer -= Time.deltaTime;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // ======================
        // MINI UI SHOW / HIDE
        // ======================
        if (bossHealthMini != null)
        {
            if (distanceToPlayer <= detectionRadius)
                bossHealthMini.ShowUI();
            else
                bossHealthMini.HideUI();
        }

        // Cancel attack if player leaves attack range
        if (isAttacking && distanceToPlayer > attackRange)
        {
            CancelAttack();
            currentState = State.Chase;
        }

        // Handle attack duration manually
        if (isAttacking)
        {
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0f)
                EndAttack();
        }

        // State switching
        if (!isAttacking)
        {
            if (distanceToPlayer <= attackRange && cooldownTimer <= 0f)
                currentState = State.Attack;
            else if (distanceToPlayer <= detectionRadius)
                currentState = State.Chase;
            else
                currentState = State.Patrol;
        }

        // Execute state
        switch (currentState)
        {
            case State.Patrol: Patrol(); break;
            case State.Chase: ChasePlayer(); break;
            case State.Attack: Attack(); break;
        }

        animator.SetBool("isWalking", agent.velocity.magnitude > 0.1f && !isAttacking);

        if (!isAttacking)
            RotateTowardsMovementDirection();
    }

    void Patrol()
    {
        if (isIdle)
        {
            idleTimer += Time.deltaTime;
            if (idleTimer >= patrolIdleTime)
            {
                SetNewPatrolPoint();
                idleTimer = 0f;
            }
            return;
        }

        if (!isPatrolling || Vector3.Distance(transform.position, patrolPoint) < 1.5f)
        {
            isIdle = true;
            isPatrolling = false;
            agent.ResetPath();
        }
    }

    void SetNewPatrolPoint()
    {
        Vector3 randomDirection = Random.insideUnitSphere * patrolRadius + transform.position;

        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, patrolRadius, NavMesh.AllAreas))
        {
            patrolPoint = hit.position;
            agent.SetDestination(patrolPoint);
            isPatrolling = true;
            isIdle = false;
        }
    }

    void ChasePlayer()
    {
        isIdle = false;
        isPatrolling = false;

        if (agent.isOnNavMesh && player != null)
            agent.SetDestination(player.position);
    }

    void Attack()
    {
        if (isAttacking) return;

        float distance = Vector3.Distance(transform.position, player.position);
        if (distance > attackRange)
        {
            currentState = State.Chase;
            return;
        }

        isAttacking = true;
        cooldownTimer = attackCooldown;
        attackTimer = attackDuration;
        agent.ResetPath();

        Vector3 lookPos = new Vector3(player.position.x, transform.position.y, player.position.z);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            Quaternion.LookRotation(lookPos - transform.position),
            Time.deltaTime * rotationSpeed);

        animator.ResetTrigger("Attack");
        animator.SetTrigger("Attack");
    }

    public void DealDamage()
    {
        if (Vector3.Distance(transform.position, player.position) <= attackRange)
        {
            playerHealth.TakeDamage(attackDamage);
        }
    }

    public void EndAttack()
    {
        isAttacking = false;
        attackTimer = 0f;
    }

    public void CancelAttack()
    {
        if (!isAttacking) return;

        isAttacking = false;
        attackTimer = 0f;
        cooldownTimer = attackCooldown;

        animator.ResetTrigger("Attack");

        if (animator.HasState(0, Animator.StringToHash("Walk")))
            animator.CrossFade("Walk", 0.1f);

        if (agent.isOnNavMesh && player != null)
            agent.SetDestination(player.position);
    }

    void RotateTowardsMovementDirection()
    {
        if (agent.velocity.sqrMagnitude > 0.1f)
        {
            Quaternion targetRotation =
                Quaternion.LookRotation(agent.velocity.normalized);

            transform.rotation =
                Quaternion.Slerp(transform.rotation, targetRotation,
                    Time.deltaTime * rotationSpeed);
        }
    }
}
