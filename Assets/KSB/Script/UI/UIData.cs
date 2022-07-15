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
    // 플레이어 수 동기화
    public TextMeshProUGUI RunnerCount;
    public TextMeshProUGUI TaggerCount;
    [SerializeField]
    int RUNNER_LIVE = 0;
    [SerializeField]
    int TAGGER_LIVE = 0;

    // 인게임 타이머 동기화
    public int totalSeconds = 0;
    public int minutes;
    public int seconds;

    public bool isStart = false;

    private void OnEnable()
    {
        GameManager.Instance.uiData = this;
        DH.UIMng.instance.UIData = this;

        RunnerCount = DH.UIMng.instance.runnerCount;
        TaggerCount = DH.UIMng.instance.taggerCount;

        minutes = 1;
        seconds = 0;
    }
    public void StartCount()
    {
        isStart = true;

        if (!(PhotonNetwork.IsMasterClient))
            return;

        DH.UIMng.instance.timer.minutesText.text = minutes.ToString();
        DH.UIMng.instance.timer.secondsText.text = seconds.ToString();

        if (10 > minutes)
        {
            DH.UIMng.instance.timer.minutesText.text = "0" + minutes;
        }

        if (10 > seconds)
        {
            DH.UIMng.instance.timer.secondsText.text = "0" + seconds;
        }

        if (minutes > 0)
        {
            totalSeconds += minutes * 60;
        }

        if (seconds > 0)
        {
            totalSeconds += seconds;
        }

        StartCoroutine(second());
    }

    public void SetTimer()
    {
        DH.UIMng.instance.timer.minutesText.text = minutes.ToString();
        DH.UIMng.instance.timer.secondsText.text = seconds.ToString();
    }

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
            stream.SendNext(totalSeconds);
            stream.SendNext(minutes); 
            stream.SendNext(seconds);
        }
        else
        {
            RUNNER_LIVE = (int)stream.ReceiveNext();
            TAGGER_LIVE = (int)stream.ReceiveNext();
            totalSeconds = (int)stream.ReceiveNext();
            minutes = (int)stream.ReceiveNext();
            seconds = (int)stream.ReceiveNext();
            SetCounting();
            SetTimer();
        }
    }

    private void Update()
    {
        if (!isStart)
            return;

        if (30 >= totalSeconds)
        {
            DH.UIMng.instance.timer.minutesText.color = Color.red;
            DH.UIMng.instance.timer.secondsText.color = Color.red;
        }

        if (seconds == 0 && minutes == 0)
        {
            minutes = -1;
            DH.UIMng.instance.timer.minutesText.enabled = false;
            DH.UIMng.instance.timer.secondsText.enabled = false;
            DH.UIMng.instance.timer.colonText.enabled = false;
            DH.UIMng.instance.timer.stopWatch.SetActive(false);
            DH.UIMng.instance.timer.timeOut.SetActive(true);

            StopAllCoroutines();

            if (PhotonNetwork.IsMasterClient)
            {
                GameManager.Instance.GameOver();
            }
            else
            {
                DH.UIMng.instance.timer.runnerWinUI.SetActive(true);
            }
        }
    }
    IEnumerator second()
    {
        yield return new WaitForSeconds(1f);
        totalSeconds--;

        if (seconds > 0)
        {
            seconds--;
        }

        if (seconds == 0 && minutes != 0)
        {
            seconds = 59;
            minutes--;
        }

        if (10 > minutes)
        {
            DH.UIMng.instance.timer.minutesText.text = "0" + minutes;
        }
        else
        {
            DH.UIMng.instance.timer.minutesText.text = minutes.ToString();
        }

        if (10 > seconds)
        {
            DH.UIMng.instance.timer.secondsText.text = "0" + seconds;
        }
        else
        {
            DH.UIMng.instance.timer.secondsText.text = seconds.ToString();
        }

        StartCoroutine(second());
    }
}
