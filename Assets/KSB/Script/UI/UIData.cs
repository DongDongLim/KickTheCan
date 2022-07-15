using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using ExitGames.Client.Photon;

public class UIData : MonoBehaviourPun, IPunObservable
{
    private void OnEnable()
    {
        GameManager.Instance.uiData = this;
        RunnerCount = DH.UIMng.instance.runnerCount;
        TaggerCount = DH.UIMng.instance.taggerCount;
        view = GetComponent<PhotonView>();
    }

    public TextMeshProUGUI RunnerCount;
    public TextMeshProUGUI TaggerCount;

    [SerializeField]
    int RUNNER_LIVE = 0;
    [SerializeField]
    int TAGGER_LIVE = 0;

    PhotonView view;

    public void SetCounting()
    {
        RunnerCount.text = RUNNER_LIVE.ToString();
        TaggerCount.text = TAGGER_LIVE.ToString();
    }

    public void RunnerCounting(int count)
    {
        RUNNER_LIVE = count;
    }

    public void TaggerCounting(int count)
    {
        TAGGER_LIVE = count;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(RUNNER_LIVE);
            stream.SendNext(TAGGER_LIVE);
        }
        else
        {
            RUNNER_LIVE = (int)stream.ReceiveNext();
            TAGGER_LIVE = (int)stream.ReceiveNext();
            SetCounting();
        }
    }

    private void OnApplicationQuit()
    {
        if (photonView.IsMine)
        {
            //PhotonNetwork.Destroy(view);
        }
    }
}
