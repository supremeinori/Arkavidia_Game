using UnityEngine;
using UnityEngine.SceneManagement;

public class GotoTutor : MonoBehaviour
{
    public void BackToMenu()
    {
        SceneManager.LoadScene("Tutor");
    }
}
