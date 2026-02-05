using UnityEngine;
using UnityEngine.AI;

public class EnemySmallAI : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Animator animator;
    public PlayerHealth playerHealth;

    [Header("Settings")]
    public float detectionRadius = 12f;
    public float attackRange = 2f;
    public float patrolRadius = 10f;
    public float attackCooldown = 2f;
    public float patrolIdleTime = 2f;
    public float rotationSpeed = 7f;
    public float attackDuration = 0.8f;

    [Header("Damage")]
    public int damage = 10;

    private NavMeshAgent agent;

    private float cooldownTimer;
    private float idleTimer;
    private float attackTimer;

    private Vector3 patrolPoint;
    private bool isPatrolling;
    private bool isIdle;
    private bool isAttacking;

    private enum State { Patrol, Chase, Attack }
    private State currentState;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (!animator)
            animator = GetComponent<Animator>();

        if (!player)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p) player = p.transform;
        }

        if (!playerHealth && player)
            playerHealth = player.GetComponent<PlayerHealth>();

        Debug.Log($"[EnemySmallAI] START on {name}");
        Debug.Log($"Player found = {player}");
        Debug.Log($"PlayerHealth found = {playerHealth}");

        SetNewPatrolPoint();
        currentState = State.Patrol;
    }

    void Update()
    {
        if (!player) return;

        cooldownTimer -= Time.deltaTime;

        float distanceToPlayer =
            Vector3.Distance(transform.position, player.position);

        Debug.Log(
            $"[EnemySmallAI] {name} | State={currentState} | Dist={distanceToPlayer:F2} | Attacking={isAttacking} | Cooldown={cooldownTimer:F2}");

        // =====================
        // CANCEL ATTACK IF PLAYER RUNS
        // =====================
        if (isAttacking && distanceToPlayer > attackRange)
        {
            Debug.Log("[EnemySmallAI] CANCEL ATTACK (player left range)");
            CancelAttack();
            currentState = State.Chase;
        }

        // =====================
        // HANDLE ATTACK TIMER
        // =====================
        if (isAttacking)
        {
            attackTimer -= Time.deltaTime;

            if (attackTimer <= 0f)
            {
                Debug.Log("[EnemySmallAI] ATTACK TIMER END");
                EndAttack();
            }
        }

        // =====================
        // STATE SWITCH
        // =====================
        if (!isAttacking)
        {
            if (distanceToPlayer <= attackRange && cooldownTimer <= 0f)
            {
                Debug.Log("[EnemySmallAI] -> SWITCH TO ATTACK");
                currentState = State.Attack;
            }
            else if (distanceToPlayer <= detectionRadius)
            {
                if (currentState != State.Chase)
                    Debug.Log("[EnemySmallAI] -> SWITCH TO CHASE");

                currentState = State.Chase;
            }
            else
            {
                if (currentState != State.Patrol)
                    Debug.Log("[EnemySmallAI] -> SWITCH TO PATROL");

                currentState = State.Patrol;
            }
        }

        // =====================
        // EXECUTE
        // =====================
        switch (currentState)
        {
            case State.Patrol: Patrol(); break;
            case State.Chase: ChasePlayer(); break;
            case State.Attack: Attack(); break;
        }

        animator.SetBool(
            "isWalking",
            agent.velocity.magnitude > 0.1f && !isAttacking);

        if (!isAttacking)
            RotateTowardsMovementDirection();
    }

    // =====================
    // PATROL
    // =====================

    void Patrol()
    {
        if (isIdle)
        {
            idleTimer += Time.deltaTime;

            if (idleTimer >= patrolIdleTime)
            {
                Debug.Log("[EnemySmallAI] Patrol idle finished -> new patrol point");
                SetNewPatrolPoint();
                idleTimer = 0f;
            }

            return;
        }

        if (!isPatrolling ||
            Vector3.Distance(transform.position, patrolPoint) < 1.5f)
        {
            Debug.Log("[EnemySmallAI] Reached patrol point -> idle");

            isIdle = true;
            isPatrolling = false;
            agent.ResetPath();
        }
    }

    void SetNewPatrolPoint()
    {
        Vector3 randomDirection =
            Random.insideUnitSphere * patrolRadius + transform.position;

        if (NavMesh.SamplePosition(
            randomDirection,
            out NavMeshHit hit,
            patrolRadius,
            NavMesh.AllAreas))
        {
            patrolPoint = hit.position;
            agent.SetDestination(patrolPoint);

            isPatrolling = true;
            isIdle = false;

            Debug.Log("[EnemySmallAI] New patrol point: " + patrolPoint);
        }
    }

    // =====================
    // CHASE
    // =====================

    void ChasePlayer()
    {
        isIdle = false;
        isPatrolling = false;

        if (agent.isOnNavMesh && player)
        {
            agent.SetDestination(player.position);
        }
    }

    // =====================
    // ATTACK
    // =====================

    void Attack()
    {
        if (isAttacking) return;

        float distance =
            Vector3.Distance(transform.position, player.position);

        if (distance > attackRange)
        {
            Debug.Log("[EnemySmallAI] Tried Attack but too far -> Chase");
            currentState = State.Chase;
            return;
        }

        Debug.Log("[EnemySmallAI] START ATTACK");

        isAttacking = true;
        cooldownTimer = attackCooldown;
        attackTimer = attackDuration;

        agent.ResetPath();

        Vector3 lookPos =
            new Vector3(
                player.position.x,
                transform.position.y,
                player.position.z);

        transform.rotation =
            Quaternion.LookRotation(lookPos - transform.position);

        animator.ResetTrigger("Attack");
        animator.SetTrigger("Attack");
    }

    // =====================
    // DAMAGE
    // =====================

    public void DealDamage()
    {
        float dist =
            Vector3.Distance(transform.position, player.position);

        Debug.Log(
            $"[EnemySmallAI] DEAL DAMAGE CALLED | Dist={dist:F2} | InRange={dist <= attackRange}");

        if (!isAttacking) return;

        if (dist <= attackRange)
        {
            Debug.Log("[EnemySmallAI] >>> PLAYER DAMAGED <<<");
            playerHealth.TakeDamage(damage);
        }
    }

    // =====================
    // END / CANCEL ATTACK
    // =====================

    void EndAttack()
    {
        Debug.Log("[EnemySmallAI] END ATTACK");

        isAttacking = false;
        attackTimer = 0f;
    }

    void CancelAttack()
    {
        Debug.Log("[EnemySmallAI] CANCEL ATTACK");

        isAttacking = false;
        attackTimer = 0f;
        cooldownTimer = attackCooldown;

        animator.ResetTrigger("Attack");

        if (agent.isOnNavMesh && player)
            agent.SetDestination(player.position);
    }

    // =====================
    // ROTATION
    // =====================

    void RotateTowardsMovementDirection()
    {
        if (agent.velocity.sqrMagnitude > 0.1f)
        {
            Quaternion targetRotation =
                Quaternion.LookRotation(agent.velocity.normalized);

            transform.rotation =
                Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    Time.deltaTime * rotationSpeed);
        }
    }
}
