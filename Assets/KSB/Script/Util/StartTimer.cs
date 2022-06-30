using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StartTimer : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI startTimer;

    public float timer;

    private void Awake() {
        startTimer.text = ((int)timer).ToString();
    }

    private void FixedUpdate() {
        timer -= Time.fixedDeltaTime;

        if ((int)timer <= 5)
            TimeSoon();
        
        startTimer.text = ((int)timer).ToString();

        if (timer <= 0)
        {
            TimerStop();
        }
    }

    public void TimeSoon()
    {
        startTimer.fontSize = 50;
        startTimer.color = Color.red;
    }

    public void TimerStop()
    {
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
