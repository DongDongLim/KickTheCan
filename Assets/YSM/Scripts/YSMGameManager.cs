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

        private void Awake()
        {
            instance = this;
            playerNumber = GetComponent<PlayerNumbering>();

        }




        public PlayerColorType GetPlayerNumberingToEnum(Player player)
        {
            return (PlayerColorType)PlayerNumberingExtensions.GetPlayerNumber(player);
        }


        public PlayerColorType GetLocalPlayerNumbering()
        {
            return (PlayerColorType)PlayerNumberingExtensions.GetPlayerNumber(PhotonNetwork.LocalPlayer);
        }


    }

}
