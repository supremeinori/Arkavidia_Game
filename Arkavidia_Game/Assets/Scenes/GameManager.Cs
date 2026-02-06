using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Pause UI")]
    public GameObject pausePanel;
    [Header("Gameplay Scenes")]
public string[] gameplayScenes;

bool pauseAllowed;


    bool isPaused;

    void Awake()
{
    if (Instance == null)
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    else if (Instance != this)
    {
        Destroy(gameObject);
    }
}
void OnEnable()
{
    SceneManager.sceneLoaded += OnSceneLoaded;
}

void OnDisable()
{
    SceneManager.sceneLoaded -= OnSceneLoaded;
}

void OnSceneLoaded(Scene scene, LoadSceneMode mode)
{
    // reset time
    isPaused = false;
    Time.timeScale = 1f;

    pauseAllowed = false;
    pausePanel = null;

    // cek apakah ini gameplay scene
    foreach (string s in gameplayScenes)
    {
        if (scene.name == s)
        {
            pauseAllowed = true;
            break;
        }
    }

    // kalau bukan gameplay → jangan cari UI
    if (!pauseAllowed)
        return;

    // cari PausePanel (boleh inactive)
    PauseUIRegister register =
        FindFirstObjectByType<PauseUIRegister>(
            FindObjectsInactive.Include);

    if (register != null)
    {
        pausePanel = register.gameObject;
        pausePanel.SetActive(false);
    }
    else
    {
        Debug.LogWarning("⚠ PausePanel not found in " + scene.name);
    }
}




    void Start()
    {
        if (pausePanel != null)
            pausePanel.SetActive(false);
    }

    void Update()
{
    if (!pauseAllowed) return;

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
    if (!pauseAllowed) return;

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
