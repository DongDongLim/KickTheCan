using System.Collections;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class Timer : MonoBehaviour
{
    public TextMeshProUGUI minutesText;
    public TextMeshProUGUI secondsText;
    public TextMeshProUGUI colonText;
    public GameObject timeOut;
    public GameObject stopWatch;
    public GameObject runnerWinUI;
    public GameObject taggerWinUI;

    public int totalSeconds = 0;
    public int minutes;
    public int sec;
        

    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            GameManager.Instance.startTime = (float)PhotonNetwork.Time;
            Hashtable prop = new Hashtable { { DH.GameData.START_TIME, GameManager.Instance.startTime } };
            PhotonNetwork.MasterClient.SetCustomProperties(prop);            
        }

        StartCoroutine(second());

    }

    public void SetZero()
    {
        StopAllCoroutines();
        minutesText.text = "00";
        secondsText.text = "00";
        minutesText.color = Color.red;
        secondsText.color = Color.red;
        StartCoroutine(GameManager.Instance.TaggerWin());
    }

    private void Update()
    {
        if (sec == 0 && minutes == 0)
        {
            minutes = -1;
            minutesText.enabled = false;
            secondsText.enabled = false;
            colonText.enabled = false;
            stopWatch.SetActive(false);
            timeOut.SetActive(true);

            StopAllCoroutines();
             
            if (PhotonNetwork.IsMasterClient)
            {
                GameManager.Instance.GameOver();
            }
            else
            {
                runnerWinUI.SetActive(true);
            }
        }
    }   

    IEnumerator second()
    {
        yield return new WaitForSeconds(1f);


        if (!PhotonNetwork.IsMasterClient)
        {
            GameManager.Instance.startTime = (float)PhotonNetwork.MasterClient.CustomProperties[DH.GameData.START_TIME];
        }
        minutes = 4 - (int)((PhotonNetwork.Time - GameManager.Instance.startTime) / 60);
        sec = 60 - (int)((PhotonNetwork.Time - GameManager.Instance.startTime) % 60);
        minutesText.text = minutes.ToString();
        secondsText.text = sec.ToString();

        if (10 > minutes)
        {
            minutesText.text = "0" + minutes;
        }

        if (10 > sec)
        {
            secondsText.text = "0" + sec;
        }

        if (minutes > 0)
        {
            totalSeconds += minutes * 60;
        }

        if (sec > 0)
        {
            totalSeconds += sec;
        }
        totalSeconds--;

        if (30 >= totalSeconds)
        {
            minutesText.color = Color.red;
            secondsText.color = Color.red;
        }

        if (sec > 0)
        {
            sec--;
        }

        if (sec == 0 && minutes != 0)
        {
            sec = 59;
            minutes--;
        }

        if (10 > minutes)
        {
            minutesText.text = "0" + minutes;
        }
        else
        {
            minutesText.text = minutes.ToString();
        }

        if (10 > sec)
        {
            secondsText.text = "0" + sec;
        }
        else
        {
            secondsText.text = sec.ToString();
        }         

        StartCoroutine(second());
    }    
}
