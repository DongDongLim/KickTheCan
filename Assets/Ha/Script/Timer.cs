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

    public int minutes;
    public int sec;

    int minutesLimit = 4;
    int secLimit = 60;
        

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
        colonText.color = Color.red;
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
        minutes = minutesLimit - (int)((PhotonNetwork.Time - GameManager.Instance.startTime) / 60);
        sec = secLimit - (int)((PhotonNetwork.Time - GameManager.Instance.startTime) % 60);
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

        if ((30 >= sec) && (minutes == 0))
        {
            colonText.color = Color.red;
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
