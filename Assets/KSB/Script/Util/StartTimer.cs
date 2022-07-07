using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StartTimer : MonoBehaviour
{
    [SerializeField] GameObject runnerCaption;
    [SerializeField] GameObject taggerCaption;

    [SerializeField] TextMeshProUGUI startTimer;

    public int timer;

    private void Start() {
        startTimer.text = timer.ToString();
        StartCoroutine(TimerCount());
    }

    private void Update() {
        if (timer <= 0)
        {
            TimerStop();
        }
    }

    IEnumerator TimerCount()
    {
        yield return new WaitForSeconds(1f);
        timer--;
        startTimer.text = timer.ToString();

        if (timer <= 5)
        {
            TimeSoon();
        }

        StartCoroutine(TimerCount());
    }

    public void TimeSoon()
    {
        startTimer.fontSize = 50;
        startTimer.color = Color.red;
    }

    public void TimerStop()
    {
        runnerCaption.SetActive(false);
        taggerCaption.SetActive(false);

        startTimer.text = "Start !";
        startTimer.color = Color.white;
        StartCoroutine(TimerStopCoroutine());

    }

    IEnumerator TimerStopCoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        gameObject.SetActive(false);
    }
}
