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

    public int RUNNER_LIFE = 0;
    public int TAGGER_LIFE = 0;

    public void SetRunner(int count)
    {
        RUNNER_LIFE = count;
        RunnerCount.text = RUNNER_LIFE.ToString();
    }

    public void SetTagger(int count)
    {
        TAGGER_LIFE = count;
        TaggerCount.text = TAGGER_LIFE.ToString();
    }
}
