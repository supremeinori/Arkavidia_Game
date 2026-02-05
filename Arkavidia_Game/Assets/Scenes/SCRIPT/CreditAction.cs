using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditActions : MonoBehaviour
{
    public void BackToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
