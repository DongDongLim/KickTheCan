using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class InConnectPanel : MonoBehaviour
{
    public PlayerSceneInfo playerSceneInfo;

    private string roomName;    

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
        if (playerSceneInfo.isLeaver)
        {
            Debug.Log("Go to GameScene");                            

            if (PhotonNetwork.InLobby) 
            {
                PhotonNetwork.LeaveLobby(); 
            }           

            roomName = playerSceneInfo.roomName;
            Debug.Log(roomName);

            Hashtable props = new Hashtable() { { GameData.PLAYER_READY, true } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);            

            playerSceneInfo.isLeaver = true;
            //PhotonNetwork.LocalPlayer.HasRejoined = true;
            //Debug.Log("Test : " + PhotonNetwork.LocalPlayer.HasRejoined);           
            PhotonNetwork.JoinRoom(roomName);           
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
