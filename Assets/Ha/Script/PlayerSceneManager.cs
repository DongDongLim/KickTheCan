using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;

public class PlayerSceneManager : MonoBehaviour
{
    public GameObject exitMenuUI;
    public PlayerSceneInfo sceneInfo;

    private void Update()
    {
        StartCoroutine("CheckEscapeButton");
        sceneInfo = GameObject.FindGameObjectWithTag("DontDestroy").GetComponent<PlayerSceneInfo>();
    }

    public void OnLobbyButton()
    {
        sceneInfo.isReturn = true;
        PhotonNetwork.LoadLevel(0);
        Debug.Log("Go to GameScene");
    }

    public void OnExitButton()
    {
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #endif
                Application.Quit();
    }

    IEnumerator CheckEscapeButton()
    {
        yield return new WaitForSeconds(0.1f);

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            exitMenuUI.SetActive(true);
        }
    }
}
