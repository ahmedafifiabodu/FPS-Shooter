using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScreenLoader : MonoBehaviour
{
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private Slider progressBar;

    public void LoadScene(int sceneIndex) => StartCoroutine(LoadScene_Co(sceneIndex));

    private IEnumerator LoadScene_Co(int sceneIndex)
    {
        progressBar.value = 0;
        loadingScreen.SetActive(true);

        AsyncOperation asyncLoad = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneIndex);
        asyncLoad.allowSceneActivation = false;

        float progress = 0;

        while (!asyncLoad.isDone)
        {
            progress = Mathf.MoveTowards(progress, asyncLoad.progress, Time.deltaTime);
            progressBar.value = progress;

            if (progress >= 0.9f)
            {
                progressBar.value = 1;
                asyncLoad.allowSceneActivation = true;
            }
            yield return null;
        }
    }
}