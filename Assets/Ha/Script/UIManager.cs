using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;

public class UIManager : MonoBehaviour
{
    public GameObject exitMenuUI;

    private void Update()
    {
        StartCoroutine("CheckEscapeButton");    
    }

    public void OnLobbyButton()
    {
        PhotonNetwork.LoadLevel(0);    
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
