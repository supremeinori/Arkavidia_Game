using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI")]
    public GameObject winPanel;

    int aliveEnemies;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (winPanel != null)
            winPanel.SetActive(false);
    }

    public void RegisterEnemy()
    {
        aliveEnemies++;
    }

    public void EnemyDied()
    {
        aliveEnemies--;

        if (aliveEnemies <= 0)
            ShowWinMenu();
    }

    void ShowWinMenu()
    {
        Time.timeScale = 0f; // pause game

        if (winPanel != null)
            winPanel.SetActive(true);
    }

    // =======================
    // BUTTON EVENTS
    // =======================

    public void RestartGame()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit Game");
    }
}

