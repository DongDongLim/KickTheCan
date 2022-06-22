using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using UnityEngine.SceneManagement;

public class PlayerSceneManager : MonoBehaviour
{
    // TODO : singleton 으로 변경
    public static PlayerSceneManager Instance { get; private set; } 

    public GameObject exitMenuUI;
    public PlayerSceneInfo sceneInfo;

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
        // TODO : 나가는 플레이어에게 관전자 모드 설정        
        sceneInfo.isRenegade = true;
        PhotonNetwork.LeaveRoom();
        //PhotonNetwork.LoadLevel(0);

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
            exitMenuUI.SetActive(true);
        }
    }
}
