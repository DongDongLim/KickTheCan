using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using System.Collections.Generic;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using DH;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager Instance { get; private set; }

    public Text infoText;
    public bool isAttack = true;
    public GameObject canCheckObj;
    public UnityAction canCheckActionTrue;
    public UnityAction canCheckActionFalse;
    public Transform[] spawnPos;
    public GameObject timer;
    public List<GameObject> playerObjList;
    public GameObject runnerWinUI;
    public GameObject taggerWinUI;

    private bool isTagger;
    private bool isPlaying = false;

    int maxTagger = 0;
    int m_deathCount = 0;



    List<Player> playerList = new List<Player>() { };

    private void Awake()
    {
        Instance = this;  
    }

    public void Start()
    {                        
        if (IsAdditionalPlayer())
        {
            Debug.Log("추가 참가");            
            ObserverMode();         
        }
        else
        {
            Debug.Log("일반 참가");
            Hashtable props = new Hashtable() { { GameData.PLAYER_LOAD, true } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        }
        if (Random.Range(0, 2) == 0)
            SoundMng.instance.PlayBGM(SoundMng.BGM_CLIP.BGM_Game1);
        else
            SoundMng.instance.PlayBGM(SoundMng.BGM_CLIP.BGM_Game2);
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
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
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
        if (changedProps.ContainsKey(GameData.PLAYER_TAGGER))
        {
            // StartSetting / DH
            // SH-> DH 전달
            // 일단 사용하지 않고 나중에 다시 수정하겠습니다.

            if (PlayersReadyLevel() == playerList.Count)
                StartCoroutine(StartCountDown());
        }
        // 러너가 킥을 찼을 때 술래의 공격불가
        object value;
        if (changedProps.TryGetValue(DH.GameData.PLAYER_ISKICK, out value))
        {
            isAttack = !(bool)value;
            if (isAttack)
            {
                canCheckActionTrue?.Invoke();
            }
            else
            {
                canCheckActionFalse?.Invoke();
            }
        }
        if (changedProps.TryGetValue(DH.GameData.PLAYER_TAGGER, out value))
        {
            if (PhotonNetwork.LocalPlayer == targetPlayer)
            {
                // TODO : 술래가 깡통을 먹음
                canCheckObj.transform.position = DH.MapSettingMng.instance.canTransform;
                canCheckObj.SetActive(true);
            }

        }

        if (changedProps.TryGetValue(GameData.PLAYER_DEAD, out value))
        {
            if ((bool)value)
            {
                CountDeath();
            }          
        }
        if(changedProps.TryGetValue(GameData.MASTER_PLAY, out value))
        {
            if(!(bool)value)
            {
                PhotonNetwork.LoadLevel(1);
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

    private int PlayersReadyLevel()
    {
        int count = 0;
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            object playerReadyLevel;

            if (p.CustomProperties.TryGetValue(GameData.PLAYER_READY, out playerReadyLevel))
            {
                if ((bool)playerReadyLevel)
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
       
        StartCoroutine(DH.MapSettingMng.instance.Setting());

        maxTagger = PhotonNetwork.PlayerList.Length / 4;
        maxTagger = (int)Mathf.Clamp(maxTagger, 1, 5);

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            playerList.Add(player);
        }

        Shuffle_List(playerList);        
        StartCoroutine("GameIsOn");

        Debug.Log(playerList.Count - maxTagger);
        Debug.Log(maxTagger);

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
                    Hashtable props = new Hashtable() { { GameData.PLAYER_TAGGER, isTagger } };
                    p.SetCustomProperties(props);
                    Debug.Log("플레이어 3명 이하! 호스트 tagger 자동설정");
                }
                else
                {
                    isTagger = false;
                    Hashtable props = new Hashtable() { { GameData.PLAYER_TAGGER, isTagger } };
                    p.SetCustomProperties(props);
                    Debug.Log("runner 설정");
                }
            }

            return;
        }

        int index = 0;

        for (int i = 0; i < playerList.Count; i++)
        {
            if (index < maxTagger)
            {
                isTagger = true;
                Hashtable props = new Hashtable() { { GameData.PLAYER_TAGGER, isTagger } };
                playerList[i].SetCustomProperties(props);
                Debug.Log("tagger 설정");
                index++;
            }
            else
            {
                isTagger = false;
                Hashtable props = new Hashtable() { { GameData.PLAYER_TAGGER, isTagger } };
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
                DH.MapSettingMng.instance.RunnerSetting("Default");
            }
        }

        isPlaying = true;

        Hashtable props = new Hashtable() { { GameData.MASTER_PLAY, isPlaying } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);

        maxTagger = PhotonNetwork.PlayerList.Length / 4;
        maxTagger = (int)Mathf.Clamp(maxTagger, 1, 5);

        SetPlayerCounting(PhotonNetwork.PlayerList.Length - maxTagger, maxTagger);
        Debug.Log(PhotonNetwork.PlayerList.Length);
        Debug.Log(maxTagger);
    }

    private void ObserverMode()
    {
        Debug.Log("ObserverMode 호출");
        Debug.Log(PhotonNetwork.CurrentRoom.PlayerCount);
              
        DH.MapSettingMng.instance.ObserverSetting(PhotonNetwork.LocalPlayer);        
    }

    IEnumerator GameIsOn()
    {
        yield return new WaitForSeconds(10f);

        Debug.Log("MasterClient : Game is On");
        isPlaying = true;

        Hashtable props = new Hashtable() { { GameData.MASTER_PLAY, isPlaying } };
        PhotonNetwork.MasterClient.SetCustomProperties(props);    
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("room에 입장");
        Debug.Log(PhotonNetwork.CurrentRoom.PlayerCount);
    }

    private bool IsAdditionalPlayer()
    {        
        Debug.Log(PhotonNetwork.CurrentRoom.Name);
        Debug.Log(PhotonNetwork.MasterClient.NickName);        
        object isStarted;       

        if (PhotonNetwork.MasterClient.CustomProperties.TryGetValue(GameData.MASTER_PLAY, out isStarted))
        {
            Debug.Log(isStarted);
            if ((bool)isStarted)
            {
                return true;
            }           
        }
        
        return false;                   
    }   

    public void GameOver()
    {
        StartCoroutine(WhoIsWinner());
        Hashtable props;
        if (PhotonNetwork.IsMasterClient)
        {
            foreach (Player p in PhotonNetwork.PlayerList)
            {
                props = new Hashtable() { { GameData.PLAYER_LOAD, false } };
                p.SetCustomProperties(props);

            }
        }

        Debug.Log("모든 유저 Load -> false");
        //PhotonNetwork.LeaveRoom();

        return;
    }

    IEnumerator WhoIsWinner()
    {
        int alivePlayer = PhotonNetwork.CurrentRoom.PlayerCount - m_deathCount - maxTagger;

        if (alivePlayer > 0)
        {
            Debug.Log("러너 승리");
            runnerWinUI.SetActive(true);
            yield return new WaitForSeconds(6f);
        }
        else
        {
            Debug.Log("술래 승리");
            taggerWinUI.SetActive(true);
            yield return new WaitForSeconds(6f);
        }

        if (PhotonNetwork.IsMasterClient)
        {
            foreach (Player p in PhotonNetwork.PlayerList)
            {
                Hashtable props = new Hashtable() { { GameData.PLAYER_LOAD, false } };
                p.SetCustomProperties(props);
                props = new Hashtable() { { GameData.MASTER_PLAY, false } };
                PhotonNetwork.LocalPlayer.SetCustomProperties(props);                
            }
        }
      
        Debug.Log("모든 유저 Load -> false");
        //PhotonNetwork.LeaveRoom();

    }

    public void CountDeath()
    {
        maxTagger = PhotonNetwork.PlayerList.Length / 4;
        maxTagger = (int)Mathf.Clamp(maxTagger, 1, 5);

        int m_iRunner = PhotonNetwork.CurrentRoom.PlayerCount - maxTagger;

        m_deathCount++;
        Debug.Log("현재 방에 있는 사람 수 : " + PhotonNetwork.CurrentRoom.PlayerCount);
        Debug.Log("죽은 러너 수 : " + m_deathCount);
        Debug.Log("러너 : " + m_iRunner);
        Debug.Log("술래 총 인원 : " + maxTagger);

        if (m_iRunner == m_deathCount)
        {
            taggerWinUI.SetActive(true);
            StartCoroutine(TaggerWin());
        }
        SetPlayerCounting(m_iRunner - m_deathCount,maxTagger);
    }

    IEnumerator TaggerWin()
    {
        yield return new WaitForSeconds(5f);

        Debug.Log("술래 승리, 게임종료");
        Hashtable props;
        if (PhotonNetwork.IsMasterClient)
        {
            foreach (Player p in PhotonNetwork.PlayerList)
            {
                props = new Hashtable() { { GameData.PLAYER_LOAD, false } };
                p.SetCustomProperties(props);
            }
            props = new Hashtable() { { GameData.MASTER_PLAY, false } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        }
        Debug.Log("모든 유저 Load -> false");
        //PhotonNetwork.LeaveRoom();
        //PhotonNetwork.LoadLevel(1);
    }

    public void SetPlayerCounting(int runner,int tagger)
    {
        // 태거 러너 인원 체크
        UIDataMng.Instance.RunnerCounting(runner);
        UIDataMng.Instance.TaggerCounting(tagger);

        Debug.Log("러너 인원 : " + runner);
        Debug.Log("태거 인원 : " + tagger);
    }
}



