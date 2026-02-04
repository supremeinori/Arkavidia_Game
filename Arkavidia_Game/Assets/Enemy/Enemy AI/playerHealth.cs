using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{   
    [Header("UI")]
    public Image healthFill;

    [Header("Health")]
    public int maxHealth = 100;
    private int currentHealth;

    [Header("Audio")]
    public AudioClip hitSFX;
    AudioSource audioSource;

    [Header("Animation")]
    public Animator animator;

    bool isDead;

    void Start()
    {
        currentHealth = maxHealth;
        audioSource = GetComponent<AudioSource>();

        if (!animator)
            animator = GetComponent<Animator>();
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (healthFill)
            healthFill.fillAmount = (float)currentHealth / maxHealth;

        if (hitSFX && audioSource)
            audioSource.PlayOneShot(hitSFX);

        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        isDead = true;

        Debug.Log("Player Died");

        if (animator)
            animator.SetTrigger("Die");

        // stop semua movement
        // SendMessage("DisableMovement", SendMessageOptions.DontRequireReceiver);

        // freeze game sementara
        // Time.timeScale = 0f;
    }
}
