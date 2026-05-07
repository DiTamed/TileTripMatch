using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class LoadingManager : MonoBehaviour
{
    public Slider progressBar;
    public Text progressText;

    void Start()
    {
        StartCoroutine(LoadScene());
    }

    IEnumerator LoadScene()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync("Home");

        operation.allowSceneActivation = false;

        float fakeProgress = 0f;

        while (!operation.isDone)
        {
            float realProgress = Mathf.Clamp01(operation.progress / 0.9f);

            fakeProgress = Mathf.MoveTowards(fakeProgress, realProgress, Time.deltaTime);

            progressBar.value = fakeProgress;
            progressText.text = (fakeProgress * 100f).ToString("F0") + "%";

            if (realProgress >= 1f && fakeProgress >= 1f)
            {
                yield return new WaitForSeconds(0.3f);
                operation.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}
