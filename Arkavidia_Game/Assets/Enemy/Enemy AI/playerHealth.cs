using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("UI")]
    public Image healthFill;

    [Header("Health")]
    public int maxHealth = 100;
    int currentHealth;

    [Header("Audio")]
    public AudioClip hitSFX;
    public AudioClip deathSFX;
    AudioSource audioSource;

    [Header("Animation")]
    public Animator animator;

    [Header("Death Settings")]
    public float deathDelay = 4f;

    bool isDead;

    // =============================
    // INIT
    // =============================

    void Start()
    {
        currentHealth = maxHealth;
        audioSource = GetComponent<AudioSource>();

        if (!animator)
            animator = GetComponent<Animator>();
    }

    // =============================
    // DAMAGE
    // =============================

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

    // =============================
    // DEATH
    // =============================

    void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log("[PlayerHealth] PLAYER DEAD");

        if (deathSFX && audioSource)
            audioSource.PlayOneShot(deathSFX);

        if (animator)
            animator.SetTrigger("Die");

        // 🚫 disable collider
        Collider col = GetComponent<Collider>();
        if (col) col.enabled = false;

        // 🚫 disable movement / controller script
        MonoBehaviour move = GetComponent<MonoBehaviour>();
        // lebih bagus disable script movement spesifik kalau ada

        StartCoroutine(DeathSequence());
    }

    IEnumerator DeathSequence()
    {
        // tunggu animasi mati
        yield return new WaitForSeconds(deathDelay);

        // 🔥 serahkan ke GameManager
        GameManager gm = FindFirstObjectByType<GameManager>();
        if (gm != null)
        {
            gm.PlayerDied();
        }
    }
}
