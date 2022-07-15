using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;


public class TimeManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameObject Timer;

    bool startTimer = false;
    double timerIncrementValue;
    double startTime;
    [SerializeField] double timer = 60;
    ExitGames.Client.Photon.Hashtable CustomValue;

    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            CustomValue = new ExitGames.Client.Photon.Hashtable();
            startTime = PhotonNetwork.Time;
            startTimer = true;
            CustomValue.Add("StartTime", startTime);
            PhotonNetwork.CurrentRoom.SetCustomProperties(CustomValue);
            Debug.Log("startTime : " + startTime);            
        }
        else
        {
            startTime = double.Parse(PhotonNetwork.CurrentRoom.CustomProperties["StartTime"].ToString());
            startTimer = true;
            Debug.Log("startTime : " + startTime);
        }
    }

    void Update()
    {
        if (!startTimer)
        {
            return;
        }

        timerIncrementValue = PhotonNetwork.Time - startTime;
     
        if (timerIncrementValue >= timer)
        {
            Debug.Log("타임 아웃");
        }        
    }
}
