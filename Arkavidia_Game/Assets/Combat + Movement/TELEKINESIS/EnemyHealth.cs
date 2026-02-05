using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    float currentHealth;

    [Header("UI")]
    public Image healthFill;
    public RectTransform healthBar;

    [Header("Audio")]
    public AudioClip hitSFX;
    public AudioClip deathSFX; // 🔥 sound mati
    AudioSource audioSource;

    Camera cam;
    bool isDead;

    void Start()
    {
        currentHealth = maxHealth;
        cam = Camera.main;
        audioSource = GetComponent<AudioSource>();

        UpdateHealthUI();
    }

    void Update()
    {
        FaceCamera();
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

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
        if (isDead) return;
        isDead = true;

        // play death sound
        if (deathSFX && audioSource)
            audioSource.PlayOneShot(deathSFX);

        // delay destroy supaya sound kedengeran
        StartCoroutine(DestroyAfterDelay(1.5f));
    }

    IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}
