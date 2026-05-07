using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void PlayGame()
    {
        AudioManager.Instance.PlayClickSound();
        SceneManager.LoadScene("GamePlay");

    }

    public void ExitGame()
    {
        AudioManager.Instance.PlayClickSound();
        Application.Quit();

        Debug.Log("Exit Game");
    }
}
