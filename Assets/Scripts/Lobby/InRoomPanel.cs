using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System.Collections.Generic;
using Photon.Realtime;
using ExitGames.Client.Photon;
using Photon.Pun.UtilityScripts;

public class InRoomPanel : MonoBehaviour
{
    public GameObject playerListContent;
    public Button startGameButton;
    public GameObject playerEntryPrefab;


    public Button readyGameButton;
    public PlayerEntry playerEntry;
    public Sprite masterImage;
    private Dictionary<int, GameObject> playerListEntries;
    PhotonView photonView;



    //강퇴기능 관련 변수
    public Button kickGameButton;
    public PlayerEntry selectedPlayerEntry;
    [SerializeField] private GameObject checkKickPanel;
    [SerializeField] private Button yesButton;
    [SerializeField] private Button noButton;
    [SerializeField] private Text playerNickName;


    /// <summary>
    /// YSM : 2022.06.16 플레이어 입장시 컬러 색상 변경을 위해 수정함
    /// </summary>
    private void OnEnable()
    {
        playerEntry = playerEntryPrefab.GetComponent<PlayerEntry>();
        photonView = GetComponent<PhotonView>();

        if (playerListEntries == null)
        {
            playerListEntries = new Dictionary<int, GameObject>();
        }

        SetMasterOrUserCilent();

        foreach (Player p in PhotonNetwork.PlayerList)
        {
            GameObject entry = Instantiate(playerEntryPrefab);

            PlayerEntry playerEntry = entry.GetComponent<PlayerEntry>();  //ysm
            playerEntry.playerColor.color = YSM.ColorTransform.EnumToColor((YSM.PlayerColorType)p.GetPlayerNumber()); //ysm
            playerEntry.SetNumbering(p.GetPlayerNumber()); //ysm


            entry.transform.SetParent(playerListContent.transform);
            entry.transform.localScale = Vector3.one;
            entry.GetComponent<PlayerEntry>().Initialize(p.ActorNumber, p.NickName);






            object isPlayerReady;
            if (p.CustomProperties.TryGetValue(GameData.PLAYER_READY, out isPlayerReady))
            {
                entry.GetComponent<PlayerEntry>().SetPlayerReady((bool)isPlayerReady);
            }

            playerListEntries.Add(p.ActorNumber, entry);
        }

        startGameButton.interactable = CheckPlayersReady();

        Hashtable props = new Hashtable
        {
            {GameData.PLAYER_LOAD, false}
        };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);

        PlayerNumbering.OnPlayerNumberingChanged += DetectPlayerNumberingChanged;


        checkKickPanel.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        foreach (GameObject entry in playerListEntries.Values)
        {
            Destroy(entry.gameObject);
        }

        playerListEntries.Clear();
        playerListEntries = null;


        Hashtable props = new Hashtable() { { GameData.PLAYER_READY, false } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        Hashtable prop1 = PhotonNetwork.LocalPlayer.CustomProperties;
        object test;
        prop1.TryGetValue(GameData.PLAYER_READY, out test);
        PlayerNumbering.OnPlayerNumberingChanged -= DetectPlayerNumberingChanged;


    }

    public void OnLeaveRoomClicked()
    {

        PhotonNetwork.LeaveRoom();
    }

    public void OnStartGameButtonClicked()
    {
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;
        PhotonNetwork.LoadLevel(1);
    }

    public void ReadyGameButtonClicked()
    {
        playerEntry.OnReadyButtonClicked();
    }


    private bool CheckPlayersReady()
    {

        int cnt = 0;
        if (!PhotonNetwork.IsMasterClient)
        {
            return false;
        }

        foreach (Player p in PhotonNetwork.PlayerList)
        {
            if (p.IsMasterClient)
                continue;

            cnt++;
            object isPlayerReady;

            if (p.CustomProperties.TryGetValue(GameData.PLAYER_READY, out isPlayerReady))
            {
                if (!(bool)isPlayerReady)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        return true;
    }

    public void LocalPlayerPropertiesUpdated()
    {
        startGameButton.interactable = CheckPlayersReady();
    }

    public void OnPlayerEnteredRoom(Player newPlayer)
    {

        GameObject entry = Instantiate(playerEntryPrefab);
        entry.transform.SetParent(playerListContent.transform);
        entry.transform.localScale = Vector3.one;
        entry.GetComponent<PlayerEntry>().Initialize(newPlayer.ActorNumber, newPlayer.NickName);

        Button test = entry.GetComponent<Button>();
        test.onClick.AddListener(() => LocalPlayerEntryClicked(playerEntry));


        playerListEntries.Add(newPlayer.ActorNumber, entry);
        startGameButton.interactable = CheckPlayersReady();




    }

    public void OnPlayerLeftRoom(Player otherPlayer)
    {
        Destroy(playerListEntries[otherPlayer.ActorNumber].gameObject);
        playerListEntries.Remove(otherPlayer.ActorNumber);
        startGameButton.interactable = CheckPlayersReady();
    }

    public void OnMasterClientSwitched(Player newMasterClient)
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber == newMasterClient.ActorNumber)
        {
            startGameButton.interactable = CheckPlayersReady();
        }
    }

    public void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (playerListEntries == null)
        {
            playerListEntries = new Dictionary<int, GameObject>();
        }

        GameObject entry;
        if (playerListEntries.TryGetValue(targetPlayer.ActorNumber, out entry))
        {
            object isPlayerReady;
            if (changedProps.TryGetValue(GameData.PLAYER_READY, out isPlayerReady))
            {
                entry.GetComponent<PlayerEntry>().SetPlayerReady((bool)isPlayerReady);
            }
        }

        startGameButton.interactable = CheckPlayersReady();
    }



    



    public void SetMasterOrUserCilent()
    {
        startGameButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);
        readyGameButton.gameObject.SetActive(!PhotonNetwork.IsMasterClient);
        kickGameButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);
    }




    //추방
    public void LocalKickGameButtonClicked()
    {

        ShowKickYesNo();
        KickButtonUsable(false);
    }

    public void ShowKickYesNo()
    {
        checkKickPanel.gameObject.SetActive(true);
        playerNickName.text = selectedPlayerEntry.playerNameText.text;
    }

    public void KickYes()
    {
        KickPlayer();
        checkKickPanel.gameObject.SetActive(false);
        KickButtonUsable(false);
    }
    

    public void KickNo()
    {
        checkKickPanel.gameObject.SetActive(false);
        KickButtonUsable(false);
    }


    public void LocalPlayerEntryClicked(PlayerEntry playerEntry)
    {

        selectedPlayerEntry = playerEntry;
        KickButtonUsable(PhotonNetwork.LocalPlayer.GetPlayerNumber() != selectedPlayerEntry.GetNumbering());
        Debug.Log(selectedPlayerEntry.GetNumbering());
    }

    public void KickButtonUsable(bool isUsable)
    {
        kickGameButton.interactable = isUsable;
    }
    public void KickPlayer()
    {
        photonView.RPC("Kicked",
                       RpcTarget.All,
                       selectedPlayerEntry.GetNumbering()
                       );
    }

    [PunRPC]
    public void Kicked(int kickedPlayerNumber)
    {

        if(kickedPlayerNumber != PhotonNetwork.LocalPlayer.GetPlayerNumber())
        {
            return;
        }

        OnLeaveRoomClicked();
    }



    /// <summary>
    /// YSM 2022.06.16 event PlayerNumbering이 바뀔때마다 각자 클라이언트에서 세팅
    /// </summary>
    public void DetectPlayerNumberingChanged()
    {

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            GameObject entry;
            if (playerListEntries.TryGetValue(player.ActorNumber, out entry))
            {
                PlayerEntry playerEntry = entry.GetComponent<PlayerEntry>();
                playerEntry.playerColor.color = YSM.ColorTransform.EnumToColor((YSM.PlayerColorType)player.GetPlayerNumber());
                playerEntry.SetNumbering(player.GetPlayerNumber());
                Debug.Log(player.GetPlayerNumber());

                Button kickButton = entry.GetComponent<Button>();
                kickButton.onClick.AddListener(() => LocalPlayerEntryClicked(playerEntry));

                if (player.IsMasterClient)
                {
                    playerEntry.playerReadyImage.sprite = masterImage;
                }
            }
        }

        if (PhotonNetwork.IsMasterClient)
        {
            SetMasterOrUserCilent();
        }
    }
}


