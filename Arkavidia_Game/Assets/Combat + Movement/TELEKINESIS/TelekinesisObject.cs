using UnityEngine;

public class TelekinesisObject : MonoBehaviour
{
    public float damage = 25f;

    private Rigidbody rb;
    private bool isThrown = false;

    void Start()
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

    EnemyHealth enemy = collision.collider.GetComponent<EnemyHealth>();
    if (enemy != null)
    {
        float damage = rb.linearVelocity.magnitude * 10f;
        enemy.TakeDamage(damage);
    }

    isThrown = false;
}

}
