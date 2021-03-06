using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using UnityEngine.SceneManagement;

public class PlayerSceneManager : MonoBehaviour
{  
    public static PlayerSceneManager Instance { get; private set; } 

    public GameObject exitMenuUI;
    public PlayerSceneInfo sceneInfo;

    bool isButtonOn = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        StartCoroutine("CheckEscapeButton");
        sceneInfo = GameObject.FindGameObjectWithTag("DontDestroy").GetComponent<PlayerSceneInfo>();
    }

    public void OnLobbyButton()
    {            
        sceneInfo.roomName = PhotonNetwork.CurrentRoom.Name;
        Debug.Log(sceneInfo.roomName);                     
        PhotonNetwork.LoadLevel(1);       
        Debug.Log("Go to Lobby");
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
            isButtonOn = !isButtonOn;
            exitMenuUI.SetActive(isButtonOn);
        }
    }
}
