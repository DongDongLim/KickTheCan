using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class InConnectPanel : MonoBehaviour
{
    public PlayerSceneInfo playerSceneInfo;

    private void Start()
    {
        playerSceneInfo = GameObject.FindGameObjectWithTag("DontDestroy").GetComponent<PlayerSceneInfo>();
    }

    public void OnCreateRoomButtonClicked()
    {
        LobbyManager.instance.SetActivePanel(LobbyManager.PANEL.CreateRoom);
    }

    public void OnRandomMatchingButtonClicked()
    {        
        if (playerSceneInfo.isRenegade)
        {
            Debug.Log("Go to GameScene"); 
            // TODO : 자신이 나왔던 방으로 들어가야된다.
            // 버그의 원인은 자신이 나왔던 방에 들어가는 것이 아닌 혼자 씬 이동만 했기 때문인 것같다.
            PhotonNetwork.LoadLevel(1);           
            return;
        }
        else
        {
            PhotonNetwork.JoinRandomRoom();
        }      
    }

    public void OnLobbyButtonClicked()
    {
        PhotonNetwork.JoinLobby();
        LobbyManager.instance.SetActivePanel(LobbyManager.PANEL.Lobby);
    }

    public void OnLogoutButtonClicked()
    {
        PhotonNetwork.Disconnect();
        LobbyManager.instance.SetActivePanel(LobbyManager.PANEL.Login);
    }
}
