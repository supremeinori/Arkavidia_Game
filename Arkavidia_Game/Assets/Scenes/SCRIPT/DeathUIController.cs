using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class DeathUIController : MonoBehaviour
{
    [Header("Scene To Load")]
    public string sceneName = "AlamBarzah";

    [Header("Fade")]
    public CanvasGroup fadeCanvas;
    public float fadeDuration = 1f;

    // 🔘 DIPANGGIL BUTTON
    public void TryAgain()
    {
        StartCoroutine(FadeAndLoad());
    }

    IEnumerator FadeAndLoad()
    {
        // pastikan game jalan
        Time.timeScale = 1f;

        if (fadeCanvas != null)
        {
            float t = 0f;

            while (t < fadeDuration)
            {
                t += Time.unscaledDeltaTime;
                fadeCanvas.alpha = t / fadeDuration;
                yield return null;
            }

            fadeCanvas.alpha = 1f;
        }

        SceneManager.LoadScene(sceneName);
    }
}
