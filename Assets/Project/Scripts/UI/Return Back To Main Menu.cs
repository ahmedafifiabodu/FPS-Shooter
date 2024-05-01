using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnBackToMainMenu : MonoBehaviour
{
    private AsyncOperation asyncLoad;

    public void ReturnToMainmenu()
    {
        if (asyncLoad != null)
        {
            asyncLoad.allowSceneActivation = true;
        }

        asyncLoad = SceneManager.LoadSceneAsync(0);
    }
}