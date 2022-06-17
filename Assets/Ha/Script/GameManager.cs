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
                SetTagger();      
                StartCoroutine(StartCountDown());
            }
            else
            {
                PrintInfo("wait players " + PlayersLoadLevel() + " / " + PhotonNetwork.PlayerList.Length);
            }
        }
        else if (changedProps.ContainsKey(GameData.PLAYER_TAGGER))
        {
            // TODO : 배정 체크
            CheckTagger();
            // TODO : 플레이어 전부 생성
            CreatePlayer();
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

        //player = PhotonNetwork.Instantiate("PlayerModel", spawnPos[playerNumber].position, spawnPos[playerNumber].rotation, 0);
        //PhotonNetwork.Instantiate("PlayerModel", spawnPos[playerNumber].position, spawnPos[playerNumber].rotation, 0);

        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(DH.MapSettingMng.instance.Setting());
        }

        CreatePlayer();       
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

    private void SetTagger()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }

        List<int> playerList = new List<int>() { };

        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            playerList.Add(i + 1);
        }

        Shuffle_List(playerList);

        maxTagger = PhotonNetwork.PlayerList.Length / 4;
        maxTagger = (int)Mathf.Clamp(maxTagger, 1, Mathf.Infinity);
        // TODO : 3명 이하면 host가 술래

        for (int i = 0; i < maxTagger; i++)
        {
            int taggerNumber = playerList[i];

            foreach (Player p in PhotonNetwork.PlayerList)
            {
                if (p.ActorNumber == taggerNumber)
                {
                    isTagger = true;
                    ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable() { { GameData.PLAYER_TAGGER, isTagger } };
                    p.SetCustomProperties(props);                 
                    Debug.Log("술래 설정");
                }
                else
                {                    
                    Debug.Log("러너 설정");
                }
            }
        }
    }

    private void CreatePlayer()
    {
        // TODO : 술래 생성        
        if (PhotonNetwork.IsMasterClient)
        {
            DH.MapSettingMng.instance.TaggerSetting(PhotonNetwork.LocalPlayer);
        }
        // TODO : 러너 생성
        else if ()
        {
            DH.MapSettingMng.instance.RunnerSetting(PhotonNetwork.LocalPlayer);
        }
    }

    private void CheckTagger()
    {
        object isTagger;

        GameObject[] bots = GameObject.FindGameObjectsWithTag("Player");
        foreach(GameObject bot in bots)
        {
            PlayerController controller = bot.GetComponent<PlayerController>();
            PhotonView pv = controller.GetComponent<PhotonView>();

            if (pv.Owner.CustomProperties.TryGetValue(GameData.PLAYER_TAGGER, out isTagger))
            {
                if ((bool)isTagger)
                {
                    controller.light.SetActive(true);
                }
            }
        }            
    }
}
