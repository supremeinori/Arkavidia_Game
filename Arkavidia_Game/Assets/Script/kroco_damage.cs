// using UnityEngine;

// public class EnemySimpleDamage : MonoBehaviour
// {
//     public PlayerHealth playerHealth;

//     public float attackRange = 2f;
//     public float attackCooldown = 2f;
//     public int damage = 10;

//     float cooldownTimer;

//     void Start()
//     {
//         if (!playerHealth)
//         {
//             playerHealth = FindObjectOfType<PlayerHealth>();
//         }

//         if (!playerHealth)
//         {
//             Debug.LogError("[EnemySimpleDamage] PlayerHealth NOT FOUND!");
//         }
//     }

//     void Update()
//     {
//         cooldownTimer -= Time.deltaTime;

//         if (!playerHealth) return;

//         float dist =
//             Vector3.Distance(transform.position,
//                              playerHealth.transform.position);

//         if (dist <= attackRange && cooldownTimer <= 0f)
//         {
//             Debug.Log("[EnemySimpleDamage] HIT PLAYER");

//             playerHealth.TakeDamage(damage);
//             cooldownTimer = attackCooldown;
//         }
//     }
// }
