using UnityEngine;

public class TelekinesisObject : MonoBehaviour
{
    public float damageMultiplier = 10f;

    Rigidbody rb;
    bool isThrown;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void SetThrown(bool value)
    {
        isThrown = value;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!isThrown) return;

        Debug.Log("Telekinesis hit: " + collision.collider.name);

        float speed = rb.linearVelocity.magnitude;
        float finalDamage = speed * damageMultiplier;

        // ===== BOSS =====
        BossHealth boss =
            collision.collider.GetComponentInParent<BossHealth>();

        if (boss != null)
        {
            Debug.Log("[TK] Hit Boss for " + finalDamage);
            boss.TakeDamage(finalDamage);
            isThrown = false;
            return;
        }

        // ===== ENEMY NORMAL =====
        EnemyHealth enemy =
            collision.collider.GetComponentInParent<EnemyHealth>();

        if (enemy != null)
        {
            Debug.Log("[TK] Hit Enemy for " + finalDamage);
            enemy.TakeDamage(finalDamage);
            isThrown = false;
            return;
        }

        // kalau nabrak tembok dll
        isThrown = false;
    }
}
