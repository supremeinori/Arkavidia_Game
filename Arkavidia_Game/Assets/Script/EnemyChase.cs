using UnityEngine;
using UnityEngine.AI;

public class EnemyChase : MonoBehaviour
{
    [Header("Target")]
    public Transform player;

    [Header("Ranges")]
    public float chaseDistance = 15f;
    public float attackRange = 2.2f;

    [Header("Attack")]
    public float attackCooldown = 2f;
    public int damage = 10;

    [Header("Animation")]
    public Animator animator;

    NavMeshAgent agent;
    PlayerHealth playerHealth;

    float cooldownTimer;
    bool isAttacking;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (!animator)
            animator = GetComponent<Animator>();

        if (!player)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p)
                player = p.transform;
        }

        if (player)
            playerHealth = player.GetComponent<PlayerHealth>();
    }

    void Update()
    {
        if (!player || !playerHealth) return;

        cooldownTimer -= Time.deltaTime;

        float dist = Vector3.Distance(transform.position, player.position);

        // ===== ATTACK MODE =====
        if (dist <= attackRange && cooldownTimer <= 0f)
        {
            agent.isStopped = true;

            if (!isAttacking)
            {
                isAttacking = true;
                animator.SetBool("isWalking", false);
                animator.SetTrigger("Attack");
            }

            return;
        }

        // ===== CHASE MODE =====
        if (dist <= chaseDistance)
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);

            animator.SetBool("isWalking", true);
        }
        else
        {
            agent.isStopped = true;
            animator.SetBool("isWalking", false);
        }
    }

    // ===== DAMAGE EVENT =====
    public void DealDamage()
    {
        if (!playerHealth) return;

        float dist = Vector3.Distance(transform.position, player.position);

        if (dist <= attackRange)
        {
            playerHealth.TakeDamage(damage);
        }
    }

    // ===== END ATTACK EVENT =====
    public void EndAttack()
    {
        isAttacking = false;
        cooldownTimer = attackCooldown;

        agent.isStopped = false;
        agent.ResetPath();
    }
}
