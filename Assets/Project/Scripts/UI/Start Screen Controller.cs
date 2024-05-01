using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StartScreenController : MonoBehaviour
{
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private Slider progressBar;

    public void NewGame() => StartCoroutine(LoadScene_Co(1, SetTime, SetHealth));

    public void ResumeGame() => StartCoroutine(LoadScene_Co(1, OnSceneLoaded, SetTime));

    private void OnSceneLoaded(AsyncOperation asyncLoad) => PlayerData.Instance.LoadPlayerData();

    private void SetTime(AsyncOperation asyncLoad) => Time.timeScale = 1;

    private void SetHealth(AsyncOperation asyncLoad) => PlayerHealth.Instance.SetHealth(PlayerHealth.Instance.GetCurrentHealth());

    public void QuitGame() => Application.Quit();

    private IEnumerator LoadScene_Co(int sceneIndex, Action<AsyncOperation> onSceneLoaded, Action<AsyncOperation> setTime)
    {
        progressBar.value = 0;
        loadingScreen.SetActive(true);

        AsyncOperation asyncLoad = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneIndex);
        asyncLoad.allowSceneActivation = false;

        asyncLoad.completed += onSceneLoaded;
        asyncLoad.completed += setTime;

        while (!asyncLoad.isDone)
        {
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            progressBar.value = progress;

            if (asyncLoad.progress >= 0.9f)
            {
                asyncLoad.allowSceneActivation = true;
            }
            yield return null;
        }

        asyncLoad.completed -= onSceneLoaded;
        asyncLoad.completed -= setTime;
    }
}