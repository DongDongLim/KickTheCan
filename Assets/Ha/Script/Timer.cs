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

    public void SetZero()
    {
        StopAllCoroutines();
        minutesText.text = "0";
        secondsText.text = "0";
        minutesText.color = Color.red;
        secondsText.color = Color.red;
        StartCoroutine(GameManager.Instance.TaggerWin());
    }
}
