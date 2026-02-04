using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    float currentHealth;

    [Header("UI")]
    public Image healthFill;
    public RectTransform healthBar;
    [Header("Audio")]
public AudioClip hitSFX;
AudioSource audioSource;


    Camera cam;

    void Start()
{
    currentHealth = maxHealth;
    cam = Camera.main;

    GameManager.Instance.RegisterEnemy();

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

    if (hitSFX && audioSource)
        audioSource.PlayOneShot(hitSFX);

    if (currentHealth <= 0)
        Die();
}


    void UpdateHealthUI()
    {
        if (healthFill != null)
            healthFill.fillAmount = currentHealth / maxHealth;
    }

    void FaceCamera()
    {
        if (healthBar != null && cam != null)
            healthBar.forward = cam.transform.forward;
    }

    void Die()
{
    GameManager.Instance.EnemyDied();
    Destroy(gameObject);
}
}
