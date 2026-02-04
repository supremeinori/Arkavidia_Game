using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class CreditMusicFade : MonoBehaviour
{
    public AudioSource music;
    public float fadeInDuration = 2f;
    public float fadeOutDuration = 2f;

    void Start()
    {
        music.volume = 0f;
        music.Play();
        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        float t = 0f;
        while (t < fadeInDuration)
        {
            t += Time.deltaTime;
            music.volume = Mathf.Lerp(0f, 1f, t / fadeInDuration);
            yield return null;
        }
        music.volume = 1f;
    }

    public void FadeOutAndBack(string sceneName)
    {
        StartCoroutine(FadeOut(sceneName));
    }

    IEnumerator FadeOut(string sceneName)
    {
        float startVolume = music.volume;
        float t = 0f;

        while (t < fadeOutDuration)
        {
            t += Time.deltaTime;
            music.volume = Mathf.Lerp(startVolume, 0f, t / fadeOutDuration);
            yield return null;
        }

        music.Stop();
        SceneManager.LoadScene(sceneName);
    }
}
