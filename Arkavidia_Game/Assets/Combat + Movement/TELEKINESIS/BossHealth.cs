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
    public AudioClip deathSFX; // 🔥 sound mati boss
    AudioSource audioSource;

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

    void Start()
    {
        currentHealth = maxHealth;
        audioSource = GetComponent<AudioSource>();

        Debug.Log($"[BossHealth] START HP = {currentHealth}");

        if (bossUIRoot)
            bossUIRoot.SetActive(false);

        if (bossHealthFill)
            bossHealthFill.fillAmount = 1f;
    }

    public void TakeDamage(float damage)
    {
        Debug.Log($"[BossHealth] TakeDamage called with {damage}");

        float before = currentHealth;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log($"[BossHealth] HP: {before} -> {currentHealth}");

        if (bossHealthFill)
            bossHealthFill.fillAmount = currentHealth / maxHealth;

        if (hitSFX && audioSource)
            audioSource.PlayOneShot(hitSFX);

        if (currentHealth <= 0)
            Die();
    }

    void Die()
{
    Debug.Log("[BossHealth] BOSS DEAD");

    if (deathSFX && audioSource)
        audioSource.PlayOneShot(deathSFX);

    HideUI();

    // 🔥 PANGGIL GAME MANAGER
    GameManager gm = FindFirstObjectByType<GameManager>();
    if (gm != null)
    {
        gm.PlayerWon();
    }

    Destroy(gameObject);
}
}
