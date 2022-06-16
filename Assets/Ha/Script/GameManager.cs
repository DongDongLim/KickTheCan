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

        PhotonNetwork.Instantiate("PlayerModel", spawnPos[playerNumber].position, spawnPos[playerNumber].rotation, 0);

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
            Debug.Log("���� �ƴ�");
            return;
        }

        Debug.Log("������");
        // ���� ���ϱ� 
        // [����] 1:3 - ����:����
        // ���� MAX : 5��

        // TODO : ���� ���� ����      

        List<int> playerList = new List<int>() { };

        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            playerList.Add(i + 1);
        }

        Shuffle_List(playerList);

        maxTagger = PhotonNetwork.PlayerList.Length / 4;
        Debug.Log("���� ���� : " + maxTagger);

        for (int i = 0; i < maxTagger; i++)
        {
            int taggerNumber = playerList[i];

            foreach (Player p in PhotonNetwork.PlayerList)
            {
               if (p.ActorNumber + 1 == taggerNumber)
                {
                    isTagger = true;
                    ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable() { { Tagger.PLAYER_TAGGER, isTagger } };
                    PhotonNetwork.LocalPlayer.SetCustomProperties(props);
                    Debug.Log("������");
                }
                Debug.Log("�÷��̾� �ѹ�" + p.GetPlayerNumber());
            }     
        }

        for (int i = 0; i < playerList.Count; i++)
        {
            Debug.Log("���õ� " + playerList[i]);
        }
    }

    public static void Shuffle_List<T>(List<T> list)
    {
        int random1;
        int random2;
        T tmp;

        for (int index = 0; index < list.Count; ++index)
        {
            random1 = UnityEngine.Random.Range(1, list.Count);
            random2 = UnityEngine.Random.Range(1, list.Count);

            tmp = list[random1];
            list[random1] = list[random2];
            list[random2] = tmp;
        }
    }

}
