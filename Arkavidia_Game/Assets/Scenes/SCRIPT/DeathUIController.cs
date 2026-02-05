using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class DeathUIController : MonoBehaviour
{
    [Header("Scene To Load")]
    public string sceneName;

    [Header("Fade")]
    public CanvasGroup fadeCanvas; // UI hitam untuk fade
    public float fadeDuration = 1f;

    // 🔘 DIPANGGIL OLEH BUTTON
    public void TryAgain()
    {
        StartCoroutine(FadeAndLoad());
    }

    // 🌑 COROUTINE FADE + LOAD
    IEnumerator FadeAndLoad()
    {
        Time.timeScale = 1f;

        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            fadeCanvas.alpha = t / fadeDuration;
            yield return null;
        }

        fadeCanvas.alpha = 1f;
        SceneManager.LoadScene(sceneName);
    }
}
