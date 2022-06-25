using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class CreateRoomPanel : MonoBehaviour
{
    public InputField roomNameInputField;
    public InputField maxPlayersInputField;

    public void OnCreateRoomCancelButtonClicked()
    {
        LobbyManager.instance.SetActivePanel(LobbyManager.PANEL.Connect);
    }

    public void OnCreateRoomConfirmButtonClicked()
    {
        string roomName = roomNameInputField.text;

        if (roomName == "")
            roomName = "Room" + Random.Range(1000, 10000);

        byte maxPlayer = byte.Parse(maxPlayersInputField.text);
        maxPlayer = (byte)Mathf.Clamp(maxPlayer, 1, 20);

        RoomOptions options = new RoomOptions { MaxPlayers = maxPlayer };
        options.PlayerTtl = 30000;
        PhotonNetwork.CreateRoom(roomName, options, null);

        //EnterRoomParams enterRoomParams = new EnterRoomParams { };
    }
}
