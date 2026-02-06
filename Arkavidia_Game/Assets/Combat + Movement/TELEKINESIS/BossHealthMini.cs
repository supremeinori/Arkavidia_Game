using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BossHealthMini : MonoBehaviour
{
    [Header("Health")]
    public float maxHealth = 500f;
    float currentHealth;

    [Header("UI (Screen)")]
    public Image bossHealthFill;
    public GameObject bossUIRoot;

    [Header("Audio")]
    public AudioClip hitSFX;
    public AudioClip deathSFX;
    AudioSource audioSource;

    [Header("Death FX")]
    public float deathDelay = 2.5f;
    public GameObject deathVFXPrefab;

    bool isDead;

    void Start()
    {
        currentHealth = maxHealth;
        audioSource = GetComponent<AudioSource>();

        Debug.Log($"[BossHealthMini] START HP = {currentHealth}");

        if (bossUIRoot)
            bossUIRoot.SetActive(false);

        if (bossHealthFill)
            bossHealthFill.fillAmount = 1f;
    }

    public void ShowUI()
    {
        if (bossUIRoot && !bossUIRoot.activeSelf)
            bossUIRoot.SetActive(true);
    }

    public void HideUI()
    {
        if (bossUIRoot && bossUIRoot.activeSelf)
            bossUIRoot.SetActive(false);
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        Debug.Log($"[BossHealthMini] TakeDamage called with {damage}");

        float before = currentHealth;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log($"[BossHealthMini] HP: {before} -> {currentHealth}");

        if (bossHealthFill)
            bossHealthFill.fillAmount = currentHealth / maxHealth;

        if (hitSFX && audioSource)
            audioSource.PlayOneShot(hitSFX);

        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log("[BossHealthMini] DEAD");

        if (deathSFX && audioSource)
            audioSource.PlayOneShot(deathSFX);

        HideUI();

        Collider col = GetComponent<Collider>();
        if (col) col.enabled = false;

        if (deathVFXPrefab)
            Instantiate(deathVFXPrefab, transform.position, Quaternion.identity);

        StartCoroutine(DeathSequence());
    }

    IEnumerator DeathSequence()
    {
        yield return new WaitForSeconds(deathDelay);

        Destroy(gameObject);
    }
}
