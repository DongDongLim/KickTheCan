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

    private PlayerSceneInfo playerSceneInfo;
    private bool isTagger;
    private bool isRebuild = false;
    
    int m_maxTagger = 0;

    List<Player> playerList = new List<Player>() { };

    private void Awake()
    {
        Instance = this;
    }

    public void Start()
    {
        playerSceneInfo = GameObject.FindGameObjectWithTag("DontDestroy").GetComponent<PlayerSceneInfo>();
        if (true == playerSceneInfo.isRenegade || true == playerSceneInfo.isObserver)
        {
            isRebuild = true;
        }
        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable() { { GameData.PLAYER_LOAD, true } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }

    #region PHOTON CALLBACK

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("Disconnected : " + cause.ToString());
        SceneManager.LoadScene(0);
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene(0);
        //PhotonNetwork.Disconnect();
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        Debug.Log("propertiesUpdate access");
        if (changedProps.ContainsKey(GameData.PLAYER_LOAD))
        {
            if (CheckAllPlayerLoadLevel())
            {
                if (!playerSceneInfo.isRenegade)
                {
                    SetTagger();
                    StartCoroutine(StartCountDown());                    
                }           
                else
                {
                    CreatePlayer(); 
                }
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
        Debug.Log("SetTagger access");
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }               

        if (isRebuild)
        {
            Debug.Log(isRebuild);
            StartCoroutine(DH.MapSettingMng.instance.Setting());
            return;
        }

        StartCoroutine(DH.MapSettingMng.instance.Setting());

        m_maxTagger = PhotonNetwork.PlayerList.Length / 4;
        m_maxTagger = (int)Mathf.Clamp(m_maxTagger, 1, 5);       

        foreach (Player  player in PhotonNetwork.PlayerList)
        {
            playerList.Add(player);
        }

        Shuffle_List(playerList);
        
        // 테스트용 tagger설정 코드
        int minPlayer = 3;      

        if (minPlayer >= PhotonNetwork.PlayerList.Length)
        {
            Debug.Log("참가 인원 : " + PhotonNetwork.PlayerList.Length);
            foreach (Player p in PhotonNetwork.PlayerList)
            {
                if (p.IsMasterClient)
                {
                    isTagger = true;
                    ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable() { { GameData.PLAYER_TAGGER, isTagger } };
                    p.SetCustomProperties(props);
                    Debug.Log("3명 이하 호스트 tagger 자동설정");
                }
                else
                {
                    isTagger = false;
                    ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable() { { GameData.PLAYER_TAGGER, isTagger } };
                    p.SetCustomProperties(props);
                    Debug.Log("runner 설정");
                }
            }

            return;
        }                    

        int index = 0;

        for (int i = 0; i < playerList.Count; i++)
        {
            if (index < m_maxTagger)
            {
                isTagger = true;
                ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable() { { GameData.PLAYER_TAGGER, isTagger } };
                playerList[i].SetCustomProperties(props);
                Debug.Log("tagger 설정");
                index++;
            }
            else
            {
                isTagger = false;
                ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable() { { GameData.PLAYER_TAGGER, isTagger } };
                playerList[i].SetCustomProperties(props);
                Debug.Log("runner 설정");                
            }
        }              
    }

    private void CreatePlayer()
    {
        Debug.Log("CreatePlayer access");        
        object isTagger;
        
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(GameData.PLAYER_TAGGER, out isTagger))
        {            
            if ((bool)isTagger)
            {
                DH.MapSettingMng.instance.TaggerSetting(PhotonNetwork.LocalPlayer);
            }
            else
            {
                DH.MapSettingMng.instance.RunnerSetting(PhotonNetwork.LocalPlayer);
            }                   
           
        }

        Debug.Log("총 tagger 인원 : " + m_maxTagger);
    }     
}



