using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Pause UI")]
    public GameObject pausePanel;

    [Header("Death UI")]
    public GameObject deathPanel;

    [Header("Win UI")]
    public GameObject winPanel;

    [Header("Optional Boss (Chapter 3 only)")]
    public BossHealth bossHealth;

    bool isPaused;
    bool isDead;
    bool hasWon;

    void Start()
    {
        if (pausePanel) pausePanel.SetActive(false);
        if (deathPanel) deathPanel.SetActive(false);
        if (winPanel) winPanel.SetActive(false);
    }

    void Update()
    {
        // =====================
        // PAUSE
        // =====================
        if (Input.GetKeyDown(KeyCode.Escape))
            TogglePause();

        // =====================
        // BOSS CHECK (SAFE)
        // =====================
        if (!hasWon && bossHealth != null)
        {
            // Boss object destroyed = DEAD
            if (bossHealth.gameObject == null)
            {
                TryTriggerWin();
            }
        }
    }

    // =========================
    // PAUSE / RESUME
    // =========================

    public void TogglePause()
    {
        if (isDead || hasWon) return;

        isPaused = !isPaused;

        Time.timeScale = isPaused ? 0f : 1f;

        if (pausePanel)
            pausePanel.SetActive(isPaused);

        if (isPaused) UnlockCursor();
        else LockCursor();
    }

    public void Resume()
    {
        if (!isPaused) return;
        TogglePause();
    }

    // =========================
    // PLAYER DEAD
    // =========================

    public void PlayerDied()
    {
        if (isDead) return;

        isDead = true;

        Time.timeScale = 0f;

        UnlockCursor();

        if (deathPanel)
            deathPanel.SetActive(true);
    }

    // =========================
    // TRY AGAIN BUTTON
    // =========================

    public void OnTryAgainPressed()
    {
        Time.timeScale = 1f;

        string scene = SceneManager.GetActiveScene().name;

        if (scene == "CHAPTER1")
        {
            SceneManager.LoadScene("AlamBarzah");
        }
        else if (scene == "CHAPTER3")
        {
            RestartGame();
        }
    }

    // =========================
    // BOSS WIN
    // =========================

    void TryTriggerWin()
    {
        if (SceneManager.GetActiveScene().name != "CHAPTER3")
            return;

        PlayerWon();
    }

    public void PlayerWon()
    {
        if (hasWon) return;

        hasWon = true;

        Time.timeScale = 0f;

        UnlockCursor();

        if (winPanel)
            winPanel.SetActive(true);
    }

    // =========================
    // RESTART
    // =========================

    public void RestartGame()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex);
    }

    // =========================
    // QUIT
    // =========================

    public void QuitGame()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene("MainMenu");
    }

    // =========================
    // CURSOR
    // =========================

    void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
