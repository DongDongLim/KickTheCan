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
    
    int m_maxTagger = 0;  

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
        Debug.Log("propertiesUpdate access");
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
            //CheckTagger();
            Debug.Log("참가 인원 : " + PhotonNetwork.PlayerList.Length);
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

        //if (PhotonNetwork.IsMasterClient)
        //{
        //    StartCoroutine(DH.MapSettingMng.instance.Setting());
        //}              
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

        List<int> playerList = new List<int>() { };

        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            playerList.Add(i + 1);
        }

        Shuffle_List(playerList);

        m_maxTagger = PhotonNetwork.PlayerList.Length / 4;
        m_maxTagger = (int)Mathf.Clamp(m_maxTagger, 1, Mathf.Infinity);       

        int minPlayer = 3;

        if (minPlayer >= PhotonNetwork.PlayerList.Length)
        {
            Debug.Log("참가 인원1 : " + PhotonNetwork.PlayerList.Length);
            foreach (Player p in PhotonNetwork.PlayerList)
            {
                if (p.IsMasterClient)
                {
                    isTagger = true;
                    ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable() { { GameData.PLAYER_TAGGER, isTagger } };
                    p.SetCustomProperties(props);                   
                    Debug.Log("3명이하이므로 호스트 술래 자동설정");    
                }
                else
                {
                    isTagger = false;
                    ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable() { { GameData.PLAYER_TAGGER, isTagger } };
                    p.SetCustomProperties(props);                   
                    Debug.Log("러너 설정");                   
                }
            }
        }
        else
        {
            for (int i = 0; i < m_maxTagger; i++)
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
                        isTagger = false;
                        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable() { { GameData.PLAYER_TAGGER, isTagger } };
                        p.SetCustomProperties(props);                        
                        Debug.Log("러너 설정");
                    }
                }
            }
        }        
    }

    private void CheckTagger()
    {
        Debug.Log("CheckTagger access");
        object isTagger;

        int countRunner = 0;
        int countTagger = 0;

        foreach (Player p in PhotonNetwork.PlayerList)
        {
            Debug.Log("참가 인원2 : " + PhotonNetwork.PlayerList.Length);
            if (p.CustomProperties.TryGetValue(GameData.PLAYER_TAGGER, out isTagger))
            {
                Debug.Log(isTagger);
                if ((bool)isTagger)
                {
                    countTagger++;
                }            
            }
            else
            {
                Debug.Log(isTagger);
                countRunner++;
            }
        }

        if (countRunner + countTagger == PhotonNetwork.PlayerList.Length)
        {           
              return;
        }
 
        Debug.Assert(countRunner + countTagger > PhotonNetwork.PlayerList.Length, "플레이어 구성원 오류");
        Debug.Log("참가 인원 : " + PhotonNetwork.PlayerList.Length);
        Debug.Log("술래 : " + countTagger);
        Debug.Log("러너 : " + countRunner);

        //GameObject[] bots = GameObject.FindGameObjectsWithTag("Player");
        //foreach (GameObject bot in bots)
        //{
        //    PlayerController controller = bot.GetComponent<PlayerController>();
        //    PhotonView pv = controller.GetComponent<PhotonView>();

        //    if (pv.Owner.CustomProperties.TryGetValue(GameData.PLAYER_TAGGER, out isTagger))
        //    {
        //        if ((bool)isTagger)
        //        {
        //            controller.light.SetActive(true);
        //        }
        //    }
        //}
    }

    private void CreatePlayer()
    {
        Debug.Log("CreatePlayer access");
        // TODO : Test -> 플레이어 생성
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
        Debug.Log("술래 : " + m_maxTagger);
    }
    
}
