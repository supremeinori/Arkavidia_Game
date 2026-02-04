using UnityEngine;
using UnityEngine.UI;

public class BossHealth : MonoBehaviour
{
    public float maxHealth = 500f;
    float currentHealth;

    [Header("UI (Screen)")]
    public Image bossHealthFill;
    public GameObject bossUIRoot;

    [Header("Audio")]
    public AudioClip hitSFX;
    AudioSource audioSource;

    void Start()
    {
        currentHealth = maxHealth;
        audioSource = GetComponent<AudioSource>();

        if (bossUIRoot)
            bossUIRoot.SetActive(true);

        if (bossHealthFill)
            bossHealthFill.fillAmount = 1f;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (bossHealthFill)
            bossHealthFill.fillAmount = currentHealth / maxHealth;

        if (hitSFX && audioSource)
            audioSource.PlayOneShot(hitSFX);

        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        if (bossUIRoot)
            bossUIRoot.SetActive(false);

        Destroy(gameObject);
    }
}
