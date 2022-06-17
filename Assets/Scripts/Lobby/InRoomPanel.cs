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

    private Dictionary<int, GameObject> playerListEntries;


    /// <summary>
    /// YSM : 2022.06.16 플레이어 입장시 컬러 색상 변경을 위해 수정함
    /// </summary>
    private void OnEnable()
    {
        
        if (playerListEntries == null)
        {
            playerListEntries = new Dictionary<int, GameObject>();
        }

        foreach (Player p in PhotonNetwork.PlayerList)
        {
            GameObject entry = Instantiate(playerEntryPrefab);

            PlayerEntry playerEntry = entry.GetComponent<PlayerEntry>();  //ysm
            playerEntry.playerColor.color = YSM.ColorTransform.EnumToColor((YSM.PlayerColorType)p.GetPlayerNumber()); //ysm



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

        startGameButton.gameObject.SetActive(CheckPlayersReady());

        Hashtable props = new Hashtable
        {
            {GameData.PLAYER_LOAD, false}
        };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);

        PlayerNumbering.OnPlayerNumberingChanged += DetectPlayerNumberingChanged;

    }

    private void OnDisable()
    {
        foreach (GameObject entry in playerListEntries.Values)
        {
            Destroy(entry.gameObject);
        }

        playerListEntries.Clear();
        playerListEntries = null;

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

        PhotonNetwork.LoadLevel("GameScene");
    }

    private bool CheckPlayersReady()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return false;
        }

        foreach (Player p in PhotonNetwork.PlayerList)
        {
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
        startGameButton.gameObject.SetActive(CheckPlayersReady());
    }

    public void OnPlayerEnteredRoom(Player newPlayer)
    {
        GameObject entry = Instantiate(playerEntryPrefab);
        entry.transform.SetParent(playerListContent.transform);
        entry.transform.localScale = Vector3.one;
        entry.GetComponent<PlayerEntry>().Initialize(newPlayer.ActorNumber, newPlayer.NickName);
        playerListEntries.Add(newPlayer.ActorNumber, entry);
        startGameButton.gameObject.SetActive(CheckPlayersReady());
    }

    public void OnPlayerLeftRoom(Player otherPlayer)
    {
        Destroy(playerListEntries[otherPlayer.ActorNumber].gameObject);
        playerListEntries.Remove(otherPlayer.ActorNumber);
        startGameButton.gameObject.SetActive(CheckPlayersReady());
    }

    public void OnMasterClientSwitched(Player newMasterClient)
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber == newMasterClient.ActorNumber)
        {
            startGameButton.gameObject.SetActive(CheckPlayersReady());
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

        startGameButton.gameObject.SetActive(CheckPlayersReady());
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
                Debug.Log(player.ActorNumber);

                PlayerEntry playerEntry = entry.GetComponent<PlayerEntry>();
                playerEntry.playerColor.color = YSM.ColorTransform.EnumToColor((YSM.PlayerColorType)player.GetPlayerNumber());
                Debug.Log(playerEntry.playerColor.color);

            }
        }
    }
}


