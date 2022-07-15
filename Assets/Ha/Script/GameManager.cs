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

    //public Text infoText;
    public bool isAttack = true;
    public GameObject canCheckObj;
    public UnityAction canCheckActionTrue;
    public UnityAction canCheckActionFalse;
    public Transform[] spawnPos;
    public GameObject timer;
    public List<GameObject> playerObjList;
    public GameObject runnerWinUI;
    public GameObject taggerWinUI;
    public float startTime;
    [SerializeField]
    Timer timerScript;

    private bool isTagger;
    private bool isPlaying = false;
    private bool isSetTagger = false;
    private bool isStart = false;

    int playerCnt = 0;
    int maxTagger = 0;
    int m_deathCount = 0;
    float readyTime = 5;


    public UIData uiData;




    List<Player> playerList = new List<Player>() { };

    private void Awake()
    {
        Instance = this;
        if (PhotonNetwork.IsMasterClient)
        {
            Hashtable props = new Hashtable { { DH.GameData.WARMUP_TIME, Time.time } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
            props = new Hashtable() { { DH.GameData.DEATH_CNT, m_deathCount } };
            PhotonNetwork.MasterClient.SetCustomProperties(props);
            PhotonNetwork.Instantiate("UIData", Vector3.zero, Quaternion.identity, 0);
        }
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
        object value;
        Debug.Log("propertiesUpdate access");
        if (changedProps.ContainsKey(GameData.PLAYER_LOAD))
        {
            if (CheckAllPlayerLoadLevel() || ((Time.time >=
                (float)PhotonNetwork.MasterClient.CustomProperties[DH.GameData.WARMUP_TIME] + readyTime) && !isStart))
            {
                if (IsAdditionalPlayer())
                {
                    isStart = true;
                    ObserverMode();
                }
                else
                    SetTagger();
                //StartCoroutine(StartCountDown());
            }
            else
            {
                PrintInfo("wait players " + PlayersLoadLevel() + " / " + PhotonNetwork.PlayerList.Length);
            }
        }
        if (changedProps.ContainsKey(GameData.PLAYER_TAGGER))
        {
            if (targetPlayer == PhotonNetwork.LocalPlayer)
            {
                isStart = true;
                StartCoroutine(StartCountDown());
                playerCnt = (int)PhotonNetwork.MasterClient.CustomProperties[DH.GameData.PLAYER_CNT];
            }
        }
        // 러너가 킥을 찼을 때 술래의 공격불가
        if (changedProps.TryGetValue(DH.GameData.PLAYER_ISKICK, out value))
        {
            isAttack = !(bool)value;
            if (isAttack)
            {
                canCheckActionTrue?.Invoke();
                m_deathCount = 0;
            }
            else
            {
                canCheckActionFalse?.Invoke();
                maxTagger = playerList.Count / 4;
                maxTagger = (int)Mathf.Clamp(maxTagger, 1, 5);

                SetPlayerCounting(playerList.Count - maxTagger, maxTagger);
            }
        }
        if (changedProps.TryGetValue(DH.GameData.PLAYER_TAGGER, out value))
        {
            if (PhotonNetwork.LocalPlayer == targetPlayer)
            {
                // TODO : 술래가 깡통을 먹음
                canCheckObj.transform.position = DH.MapSettingMng.instance.canTransform;
                Debug.Log(canCheckObj.transform.position);
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
        if (changedProps.TryGetValue(GameData.MASTER_PLAY, out value))
        {
            if (!(bool)value && PhotonNetwork.IsMasterClient)
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
        //infoText.text = info;
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
        if (isSetTagger)
            return;
        isSetTagger = true;

        StartCoroutine(DH.MapSettingMng.instance.Setting());

        maxTagger = PhotonNetwork.PlayerList.Length / 4;
        maxTagger = (int)Mathf.Clamp(maxTagger, 1, 5);

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            playerList.Add(player);
        }


        isPlaying = true;

        Hashtable props = new Hashtable() { { GameData.MASTER_PLAY, isPlaying } };
        PhotonNetwork.MasterClient.SetCustomProperties(props);
        props = new Hashtable { { DH.GameData.PLAYER_CNT, playerList.Count } };
        PhotonNetwork.MasterClient.SetCustomProperties(props);
        Shuffle_List(playerList);
        // 테스트용 tagger설정 코드
        int minPlayer = 3;

        if (minPlayer >= PhotonNetwork.PlayerList.Length)
        {
            foreach (Player p in PhotonNetwork.PlayerList)
            {
                if (p.IsMasterClient)
                {
                    isTagger = true;
                    props = new Hashtable() { { GameData.PLAYER_TAGGER, isTagger } };
                    p.SetCustomProperties(props);
                }
                else
                {
                    isTagger = false;
                    props = new Hashtable() { { GameData.PLAYER_TAGGER, isTagger } };
                    p.SetCustomProperties(props);
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
                props = new Hashtable() { { GameData.PLAYER_TAGGER, isTagger } };
                playerList[i].SetCustomProperties(props);
                index++;
            }
            else
            {
                isTagger = false;
                props = new Hashtable() { { GameData.PLAYER_TAGGER, isTagger } };
                playerList[i].SetCustomProperties(props);
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

        maxTagger = playerList.Count / 4;
        maxTagger = (int)Mathf.Clamp(maxTagger, 1, 5);

        SetPlayerCounting(playerList.Count - maxTagger, maxTagger);
    }

    private void ObserverMode()
    {
        m_deathCount = (int)PhotonNetwork.MasterClient.CustomProperties[DH.GameData.DEATH_CNT];
        DH.MapSettingMng.instance.ObserverSetting(PhotonNetwork.LocalPlayer);
    }

    public override void OnJoinedRoom()
    {
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

        int m_iRunner = playerCnt - maxTagger;
        
        m_deathCount++;
        if (PhotonNetwork.IsMasterClient)
        {
            Hashtable props = new Hashtable() { { DH.GameData.DEATH_CNT, m_deathCount } };
            PhotonNetwork.MasterClient.SetCustomProperties(props);
        }
        Debug.Log(m_deathCount + "," + m_iRunner);
        if (m_iRunner == m_deathCount)
        {
            taggerWinUI.SetActive(true);
            timerScript.SetZero();
        }
        SetPlayerCounting(m_iRunner - m_deathCount, maxTagger);
    }

    public IEnumerator TaggerWin()
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

    public void SetPlayerCounting(int runner, int tagger)
    {

        if (PhotonNetwork.IsMasterClient)
        {
            uiData.RunnerCounting(runner);
            uiData.TaggerCounting(tagger);
        }

        uiData.SetCounting();
    }
}



