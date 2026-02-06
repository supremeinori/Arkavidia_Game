using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Pause UI")]
    public GameObject pausePanel;

    bool isPaused;

    void Start()
    {
        if (pausePanel != null)
            pausePanel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    // =========================
    // PAUSE / RESUME
    // =========================

    public void TogglePause()
    {
        isPaused = !isPaused;

        Time.timeScale = isPaused ? 0f : 1f;

        if (pausePanel != null)
            pausePanel.SetActive(isPaused);

        if (isPaused)
            UnlockCursor();
        else
            LockCursor();
    }

    public void Resume()
    {
        if (!isPaused) return;

        TogglePause();
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
    // QUIT TO MENU
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
