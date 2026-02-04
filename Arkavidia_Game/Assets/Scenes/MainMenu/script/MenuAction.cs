using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuActions : MonoBehaviour
{
    // ===== PLAY =====
    public void PlayGame()
    {
        SceneManager.LoadScene("CHAPTER1"); 
        // ganti dengan nama scene game kamu
    }

    // ===== CREDIT =====
    public void OpenCredit()
    {
        SceneManager.LoadScene("CREDIT"); 
        // ganti sesuai nama scene credit
    }

    // ===== EXIT =====
    public void ExitGame()
    {
        Debug.Log("Exit Game");

        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
