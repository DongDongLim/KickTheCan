using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using System.Collections.Generic;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager Instance { get; private set; }

    public Text infoText;
    public Transform[] spawnPos;

    private bool isTagger;

    GameObject player;
    
    int maxTagger = 0;    

    private void Awake()
    {
        Instance = this;
    }

    public void Start()
    {
        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable() { { GameData.PLAYER_LOAD, true } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }

    #region PHOTON CALLBACK

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("Disconnected : " + cause.ToString());
        SceneManager.LoadScene("LobbyScene");
    }

    public override void OnLeftRoom()
    {
        PhotonNetwork.Disconnect();
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (changedProps.ContainsKey(GameData.PLAYER_LOAD))
        {
            if (CheckAllPlayerLoadLevel())
            {
                StartCoroutine(StartCountDown());
            }
            else
            {
                PrintInfo("wait players " + PlayersLoadLevel() + " / " + PhotonNetwork.PlayerList.Length);
            }
        }
        else if (changedProps.ContainsKey(GameData.PLAYER_TAGGER))
        {
            // TODO : 다시확인 할것 
            // TODO : 술래 선정이 바꼈으니 다들 술래인지 확인
            Debug.Log(targetPlayer.NickName + " 플레이어가 술래가 바뀐걸 확인했다.");
            player.GetComponent<PlayerController>().CheckTagger();
        }
    }

    #endregion PHOTON CALLBACK

    private IEnumerator StartCountDown()
    {
        PrintInfo("All Player Loaded, Start Count Down");
        yield return new WaitForSeconds(1.0f);

        for (int i = GameData.COUNTDOWN; i > 0; i--)
        {
            PrintInfo("Count Down " + i);
            yield return new WaitForSeconds(1.0f);
        }

        PrintInfo("Start Game!");  

        int playerNumber = PhotonNetwork.LocalPlayer.GetPlayerNumber();      

        player = PhotonNetwork.Instantiate("PlayerModel", spawnPos[playerNumber].position, spawnPos[playerNumber].rotation, 0);

        SetTagger();
    }

    private bool CheckAllPlayerLoadLevel()
    {
        return PlayersLoadLevel() == PhotonNetwork.PlayerList.Length;
    }

    private int PlayersLoadLevel()
    {
        int count = 0;
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            object playerLoadedLevel;
            
            if (p.CustomProperties.TryGetValue(GameData.PLAYER_LOAD, out playerLoadedLevel))
            {
                if ((bool)playerLoadedLevel)
                {
                    count++;
                }
            }
        }

        return count;
    }

    private void PrintInfo(string info)
    {
        Debug.Log(info);
        infoText.text = info;
    }

    private void SetTagger()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            Debug.Log("방장 아님");
            return;
        }

        Debug.Log("방장임");
        // 술래 정하기 
        // [비율] 1:3 - 술래:러너
        // 술래 MAX : 5명          

        List<int> playerList = new List<int>() { };

        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            playerList.Add(i + 1);
        }

        Shuffle_List(playerList);

        maxTagger = PhotonNetwork.PlayerList.Length / 4;
        maxTagger = (int)Mathf.Clamp(maxTagger, 1, Mathf.Infinity);

        for (int i = 0; i < maxTagger; i++)
        {
            int taggerNumber = playerList[i];
            Debug.Log("술래 숫자 : " + taggerNumber);

            foreach (Player p in PhotonNetwork.PlayerList)
            {
                Debug.Log("ActorNumber : " + p.ActorNumber);
                if (p.ActorNumber == taggerNumber)
                {
                    isTagger = true;
                    ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable() { { GameData.PLAYER_TAGGER, isTagger } };
                    p.SetCustomProperties(props);
                    Debug.Log("술래 : " + p.ActorNumber);
                    break;
                }                
            }     
        }

        for (int i = 0; i < playerList.Count; i++)
        {
            Debug.Log("셔플된 리스트 " + playerList[i]);
        }
    }

    public static void Shuffle_List<T>(List<T> list)
    {     
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = UnityEngine.Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

}
