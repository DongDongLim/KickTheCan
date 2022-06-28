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

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager Instance { get; private set; }

    public Text infoText;
    public bool isAttack = true;
    public GameObject canCheckObj;
    public UnityAction canCheckActionTrue;
    public UnityAction canCheckActionFalse;

    private PlayerSceneInfo playerSceneInfo;
    private bool isTagger;
    private bool isPlaying = false;

    int m_maxTagger = 0;

    List<Player> playerList = new List<Player>() { };

    private void Awake()
    {
        Instance = this;
    }

    public void Start()
    {
        playerSceneInfo = GameObject.FindGameObjectWithTag("DontDestroy").GetComponent<PlayerSceneInfo>();

        if (PhotonNetwork.LocalPlayer.HasRejoined)
        {
            Debug.Log("재 참가");
            // TODO : test - 관전자 모드 시 러너 미생성
            //Hashtable props = new Hashtable() { { GameData.PLAYER_LOAD, true } };
            //PhotonNetwork.LocalPlayer.SetCustomProperties(props);
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

        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine("GameIsOn");
        }
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

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        Debug.Log("propertiesUpdate access");
        if (changedProps.ContainsKey(GameData.PLAYER_LOAD))
        {
            if (CheckAllPlayerLoadLevel())
            {
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

        StartCoroutine(DH.MapSettingMng.instance.Setting());

        m_maxTagger = PhotonNetwork.PlayerList.Length / 4;
        m_maxTagger = (int)Mathf.Clamp(m_maxTagger, 1, 5);

        foreach (Player player in PhotonNetwork.PlayerList)
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
        // 맵 세팅은 호스트만 부탁드립니다
        //StartCoroutine(DH.MapSettingMng.instance.Setting());
        DH.MapSettingMng.instance.ObserverSetting(PhotonNetwork.LocalPlayer);
    }

    private void RejoinMode()
    {
        Debug.Log("ReJoiner Mode 호출");
        //StartCoroutine(DH.MapSettingMng.instance.Setting());
        DH.MapSettingMng.instance.RunnerSetting("Default");
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
}
