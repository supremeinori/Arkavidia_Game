using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;

    [Header("UI")]
    public Image healthFill;
    public RectTransform healthBar;
    private Camera cam;

    void Start()
    {
        currentHealth = maxHealth;
        cam = Camera.main;
        UpdateHealthUI();

    }

    void Update()
    {
        FaceCamera();
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        UpdateHealthUI();

        if (currentHealth <= 0)
            Die();
    }

    void UpdateHealthUI()
    {
        healthFill.fillAmount = currentHealth / maxHealth;
    }

    void FaceCamera()
    {
        healthBar.forward = cam.transform.forward;
    }

    void Die()
    {
        Destroy(gameObject);
    }
}
