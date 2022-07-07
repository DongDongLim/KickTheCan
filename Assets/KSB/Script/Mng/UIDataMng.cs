using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using ExitGames.Client.Photon;

public class UIDataMng : MonoBehaviourPunCallbacks
{
    public static UIDataMng Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }

    public TextMeshProUGUI RunnerCount;
    public TextMeshProUGUI TaggerCount;

    int RUNNER_LIVE = 0;
    int TAGGER_LIVE = 0;

    public void RunnerCounting(int count)
    {
        RUNNER_LIVE = count;
        RunnerCount.text = RUNNER_LIVE.ToString();
    }

    public void TaggerCounting(int count)
    {
        TAGGER_LIVE = count;
        TaggerCount.text = TAGGER_LIVE.ToString();
    }
}
