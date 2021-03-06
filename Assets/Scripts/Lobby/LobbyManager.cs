using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System.Collections.Generic;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    public static LobbyManager instance {get; private set;}

    [Header("Panel")]
    public LoginPanel loginPanel;
    public InConnectPanel inConnectPanel;
    public CreateRoomPanel createRoomPanel;
    public InLobbyPanel inLobbyPanel;
    public InRoomPanel inRoomPanel;
    //public InfoPanel infoPanel;
    public GoogleNickNamePanel nickNamePanel;

    #region UNITY

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public override void OnEnable()
    {
        base.OnEnable();
        if(PhotonNetwork.InRoom)
            SetActivePanel(PANEL.Room);
    }

    private void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        
        // TOOD : ?߰?
        if (PhotonNetwork.IsConnected)
            SetActivePanel(LobbyManager.PANEL.Connect);

        SoundMng.instance.PlayBGM(SoundMng.BGM_CLIP.BGM_Lobby);
    }

    public enum PANEL { Login, Connect, Lobby, Room, CreateRoom, NickName }
    public void SetActivePanel(PANEL panel)
    {
        loginPanel.gameObject.SetActive(panel == PANEL.Login);
        inConnectPanel.gameObject.SetActive(panel == PANEL.Connect);
        createRoomPanel.gameObject.SetActive(panel == PANEL.CreateRoom);
        inLobbyPanel.gameObject.SetActive(panel == PANEL.Lobby);
        inRoomPanel.gameObject.SetActive(panel == PANEL.Room);
        nickNamePanel.gameObject.SetActive(panel == PANEL.NickName);
    }  

    public void ShowError(string error)
    {
        //infoPanel.ShowError(error);
    }

    #endregion UNITY

    #region PHTON CALLBACK

    public override void OnConnectedToMaster()
    {
        SetActivePanel(PANEL.Connect);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        inLobbyPanel.OnRoomListUpdate(roomList);
    }

    public override void OnJoinedLobby()
    {
        inLobbyPanel.ClearRoomList();
    }

    public override void OnLeftLobby()
    {
        inLobbyPanel.ClearRoomList();
    }

    public override void OnCreatedRoom() 
    {
        // TODO : ?? ???ӽð?
        base.OnCreatedRoom();
        EnterRoomParams enterRoomParams = new EnterRoomParams { };

        Room room = PhotonNetwork.CurrentRoom;
        room.EmptyRoomTtl = 1;
        
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        SetActivePanel(PANEL.Connect);
        //infoPanel.ShowError("Create Room Failed with Error(" + returnCode + ") : " + message);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        SetActivePanel(PANEL.Connect);
        //infoPanel.ShowError("Join Room Failed with Error(" + returnCode + ") : " + message);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        string roomName = "Room " + Random.Range(1000, 10000);
        RoomOptions options = new RoomOptions { MaxPlayers = 20 };
        PhotonNetwork.CreateRoom(roomName, options, null);
    }

    public override void OnJoinedRoom()
    {
        SetActivePanel(PANEL.Room);
        Hashtable props = new Hashtable() { { GameData.PLAYER_READY, false } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }

    public override void OnLeftRoom()
    {
        Debug.Log("???? ????");
        SetActivePanel(PANEL.Connect);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        inRoomPanel.OnPlayerEnteredRoom(newPlayer);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        inRoomPanel.OnPlayerLeftRoom(otherPlayer);       
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        inRoomPanel.OnMasterClientSwitched(newMasterClient);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        inRoomPanel.OnPlayerPropertiesUpdate(targetPlayer, changedProps);
    }

    public void LocalPlayerPropertiesUpdated()
    {
        inRoomPanel.LocalPlayerPropertiesUpdated();
    }

    public void OnMoveLobbyButton()
    {

    }

    #endregion
}
