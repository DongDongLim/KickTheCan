using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace YSM
{


    public class YSMGameManager : MonoBehaviourPunCallbacks
    {

        static public YSMGameManager instance { get; private set; }
        public PlayerNumbering playerNumber;
        static private int setWidth = 1920; //화면  너비
        static private int setHeight = 1080; //화면 높이


        public DBData myData;

        private void Awake()
        {

            instance = this;
            playerNumber = GetComponent<PlayerNumbering>();

            Screen.SetResolution(setWidth, setHeight, true);
        }


        public PlayerColorType GetLocalPlayerNumbering()
        {
            return (PlayerColorType)PlayerNumberingExtensions.GetPlayerNumber(PhotonNetwork.LocalPlayer);
        }









    }

}
