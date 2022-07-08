using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScene : MonoBehaviour
{
    bool isTrue = true;
    private void FixedUpdate()
    {
        Button();
    }

    private void Button()
    {
        if (Input.anyKeyDown)
        {
            if (isTrue)
            {
                SceneManager.LoadScene(1);
                isTrue = false;
            }
        }
    }
}
