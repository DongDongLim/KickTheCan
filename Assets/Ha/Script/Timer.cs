using System.Collections;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

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

    bool isGameOver = false;
        

    private void Start()
    {
        StartCoroutine(TimeOver());
        minutesText.text = minutes.ToString();
        secondsText.text = sec.ToString();
        
        if (10 > minutes)
        {
            minutesText.text =  "0" + minutes;
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

        StartCoroutine(second());

    }  

    IEnumerator TimeOver()
    {
        while (!isGameOver)
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
            yield return new WaitForSeconds(0.1f);
        }

        StopAllCoroutines();
    }

    IEnumerator second()
    {
        yield return new WaitForSeconds(1f);
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

    public void StopTimer()
    {
        StopAllCoroutines();

        Debug.Log("Game Over 시간 멈춰!");
        isGameOver = true;

        sec = 0;
        minutes = 0;

        secondsText.text = "0" + sec;
        minutesText.text = "0" + minutes;

        minutesText.color = Color.red;
        secondsText.color = Color.red;
        colonText.color = Color.red;
    }
}
