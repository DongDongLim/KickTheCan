using UnityEngine;
using Photon.Pun;

public class InConnectPanel : MonoBehaviour
{
    public void OnCreateRoomButtonClicked()
    {
        LobbyManager.instance.SetActivePanel(LobbyManager.PANEL.CreateRoom);
    }

    public void OnRandomMatchingButtonClicked()
    {
        // TODO : �κ� �����ӽ� ���Ӿ����� �̵��ϱ� 
        if (false)
        {
            Debug.Log("Go to GameScene");
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
