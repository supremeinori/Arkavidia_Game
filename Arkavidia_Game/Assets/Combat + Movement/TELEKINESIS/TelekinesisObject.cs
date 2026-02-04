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

        EnemyHealth enemy =
            collision.collider.GetComponentInParent<EnemyHealth>();

        if (enemy != null)
        {
            float speed = rb.linearVelocity.magnitude;
            float finalDamage = speed * damageMultiplier;

            enemy.TakeDamage(finalDamage);
        }
        Debug.Log("Telekinesis hit: " + collision.collider.name);

        // supaya gak ngerusak berkali-kali
        isThrown = false;
    }
}
