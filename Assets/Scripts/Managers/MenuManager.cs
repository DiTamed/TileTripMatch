using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void PlayGame()
    {
        if (!LevelSession.TrySelectLevel(LevelSession.CurrentLevelIndex))
            LevelSession.TrySelectLevel(1);

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayClickSound();
        SceneManager.LoadScene("GamePlay");
    }

    public void PlayLevel(int levelIndex)
    {
        if (!LevelSession.TrySelectLevel(levelIndex))
        {
            Debug.LogWarning($"Level {levelIndex} could not be loaded.");
            return;
        }

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayClickSound();
        SceneManager.LoadScene("GamePlay");
    }

    public void ExitGame()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayClickSound();
        Application.Quit();

        Debug.Log("Exit Game");
    }
}
