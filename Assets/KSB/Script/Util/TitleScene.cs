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
            SceneManager.LoadScene(1);
            isTrue = false;
        }
    }
}
