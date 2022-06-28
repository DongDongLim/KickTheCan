using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public Text minutesText;
    public Text secondsText;
    public GameObject timeOut;

    public int totalSeconds = 0;
    public int minutes;
    public int sec;
        

    private void Start()
    {
        minutesText.text = minutes.ToString();
        secondsText.text = sec.ToString();
        
        if (10 > minutes)
        {
            minutesText.text =  "0" + minutes + " : ";
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

    private void Update()
    {
        if (sec == 0 && minutes == 0)
        {
            minutesText.enabled = false;
            secondsText.enabled = false;
            timeOut.SetActive(true);

            StopAllCoroutines();
        }
    }

    IEnumerator second()
    {
        yield return new WaitForSeconds(1f);
        totalSeconds--;

        if (30 >= totalSeconds)
        {
            GetComponent<Text>().color = Color.red;
            secondsText.GetComponent<Text>().color = Color.red;
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
            minutesText.text = "0" + minutes + " : ";
        }
        else
        {
            minutesText.text = minutes + " : ";
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
