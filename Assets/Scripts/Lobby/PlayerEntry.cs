using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using ExitGames.Client.Photon;

public class PlayerEntry : MonoBehaviour
{
    [Header("UI References")]
    public Text playerNameText;
    public Button playerReadyButton;
    public Image playerReadyImage;
    public Image playerFace;
    public Image playerColor;
    public int numbering;
    private int ownerId;
    private bool isPlayerReady;

    // ���� ���ϴ°�
    // ���� �÷��� �����.
    // �� �÷��� ��ġ�� �ȵǰ� ,
    // ������ �������� ���� 


    public void Start()
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber != ownerId)
        {
            playerReadyButton.gameObject.SetActive(false);
        }

        Hashtable props = new Hashtable() { { GameData.PLAYER_READY, false } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);

    }



    public void OnReadyButtonClicked()
    {
        Hashtable props = PhotonNetwork.LocalPlayer.CustomProperties;
        object isReady;
        props.TryGetValue(GameData.PLAYER_READY, out isReady);

        isPlayerReady = !(bool)isReady;
        SetPlayerReady(isPlayerReady);
        props = new Hashtable() { { GameData.PLAYER_READY, isPlayerReady } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);


        if (PhotonNetwork.IsMasterClient)
        {
            LobbyManager.instance.LocalPlayerPropertiesUpdated();
        }
    }

    public void Initialize(int playerId, string playerName)
    {
        ownerId = playerId;
        playerNameText.text = playerName;
    }

    public int GetOwnerID()
    {
        return ownerId;
    }


    public void SetNumbering(int number)
    {
        numbering = number;
    }

    public int GetNumbering()
    {
        return numbering;
    }



    /// <summary>
    /// YSM 2022.06.16
    /// Player �غ��ϰ� �������� �� ���󺯰� 
    /// </summary>
    /// <param name="playerReady"></param>
    public void SetPlayerReady(bool playerReady)
    {
        playerReadyImage.color = playerReady ? Color.green : Color.red;
    }

}
