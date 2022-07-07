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

    int m_maxTagger = 0;
    int m_deathCount = 0;



    List<Player> playerList = new List<Player>() { };

    private void Awake()
    {
        Instance = this;  
    }

    public void Start()
    {                        
        if (PhotonNetwork.LocalPlayer.HasRejoined)
        {         
            Debug.Log("재 참가");                
            RejoinMode();            
        }
        else if (IsAdditionalPlayer())
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

        timer.SetActive(true);
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

        // TODO : Text 예정
        StartCoroutine(DH.MapSettingMng.instance.Setting());

        m_maxTagger = PhotonNetwork.PlayerList.Length / 4;
        m_maxTagger = (int)Mathf.Clamp(m_maxTagger, 1, 5);

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            playerList.Add(player);
        }

        Shuffle_List(playerList);
        // TODO : (Test) StartSetting / DH
        StartCoroutine("GameIsOn");

        Debug.Log(playerList.Count - m_maxTagger);
        Debug.Log(m_maxTagger);

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
            if (index < m_maxTagger)
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
    }

    private void ObserverMode()
    {
        Debug.Log("ReEntry Mode 호출");
        Debug.Log(PhotonNetwork.CurrentRoom.PlayerCount);
              
        DH.MapSettingMng.instance.ObserverSetting(PhotonNetwork.LocalPlayer);        
    }

    private void RejoinMode()
    {
        Debug.Log("ReJoiner Mode 호출");     
        DH.MapSettingMng.instance.RunnerSetting("Default");
    }

    IEnumerator GameIsOn()
    {
        yield return new WaitForSeconds(10f);

        Debug.Log("MasterClient : Game is On");
        isPlaying = true;

        Hashtable props = new Hashtable() { { GameData.MASTER_PLAY, isPlaying } };
        PhotonNetwork.MasterClient.SetCustomProperties(props);  
        
        // TODO : 모든플레이어가 준비가 되었을 시에 게임을 시작한다.
        // 정해진 시간안에 참가하지 못한 플레이어는 추방하고 게임을 시작한다.
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("room에 입장");
        Debug.Log(PhotonNetwork.CurrentRoom.PlayerCount);

        m_maxTagger = PhotonNetwork.PlayerList.Length / 4;
        m_maxTagger = (int)Mathf.Clamp(m_maxTagger, 1, 5);

        SetPlayerCounting(playerList.Count - m_maxTagger, m_maxTagger);
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
        // 게임 종료 조건 체크 -> true -> 승자 UI 표시 -> 모두 룸으로 가기           
       

        // TODO : 모두 로비로
        if (PhotonNetwork.IsMasterClient)
        {
            //PhotonNetwork.LeaveRoom();
            //PhotonNetwork.LoadLevel(1);
            // Test 같이 시작하는 방으로 돌아가기
        }

        Debug.Log("게임 종료");
        Debug.Log("Go to Lobby");
        
        // TODO : 모든 플레이어 load, ready -> false로 바꾸기

        return;
    }

    public void WhoIsWinner()
    {
        //yield return new WaitForSeconds(2f);
        
        playerObjList = Instance.GetComponent<MapSettingMng>().playerObjList;

        foreach (GameObject player in playerObjList)
        {
            if (!player.GetComponent<PlayerScript>().isDead)
            {
                // 러너 승리
                Debug.Log("러너 승리");
                runnerWinUI.SetActive(true);
                StopAllCoroutines();
            }            
        }

        // 술래 승리
        Debug.Log("술래 승리");
        taggerWinUI.SetActive(true);
    }

    public void CountDeath()
    {
        m_maxTagger = PhotonNetwork.PlayerList.Length / 4;
        m_maxTagger = (int)Mathf.Clamp(m_maxTagger, 1, 5);

        int m_iRunner = PhotonNetwork.CurrentRoom.PlayerCount - m_maxTagger;

        m_deathCount++;
        Debug.Log("현재 방에 있는 사람 수 : " + PhotonNetwork.CurrentRoom.PlayerCount);
        Debug.Log("죽은 러너 수 : " + m_deathCount);
        Debug.Log("러너 : " + m_iRunner);
        Debug.Log("술래 총 인원 : " + m_maxTagger);

        if (m_iRunner == m_deathCount)
        {
            taggerWinUI.SetActive(true);
            StartCoroutine(TaggerWin());
        }
        SetPlayerCounting(m_iRunner - m_deathCount,m_maxTagger);
    }

    IEnumerator TaggerWin()
    {
        yield return new WaitForSeconds(5f);

        Debug.Log("술래 승리, 게임종료");

        if (PhotonNetwork.IsMasterClient)
        {
            foreach (Player p in PhotonNetwork.PlayerList)
            {
                Hashtable props = new Hashtable() { { GameData.PLAYER_LOAD, false } };
                p.SetCustomProperties(props);
            }
        }
        Debug.Log("모든 유저 Load -> false");
        PhotonNetwork.LeaveRoom();
    }

    public void SetPlayerCounting(int runner,int tagger)
    {
        // 태거 러너 인원 체크
        UIDataMng.Instance.RunnerCounting(runner);
        UIDataMng.Instance.TaggerCounting(tagger);

        Debug.Log("러너 인원 : "+runner);
        Debug.Log("태거 인원 : " + tagger);
    }
}



