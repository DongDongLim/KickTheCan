using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScene : MonoBehaviour
{
    bool isTrue = true;
    public void Button()
    {
        if (isTrue)
        {
            StartCoroutine(LoadYourAsyncScene(1));
            isTrue = false;
        }
    }

    IEnumerator LoadYourAsyncScene(int sceneNum)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneNum, LoadSceneMode.Single);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}
