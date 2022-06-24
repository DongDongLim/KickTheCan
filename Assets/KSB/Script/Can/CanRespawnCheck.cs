using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace DH
{
    public class CanRespawnCheck : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Tagger"))
            {
                Hashtable prop = new Hashtable { {GameData.PLAYER_ISKICK, false } };
                PhotonNetwork.LocalPlayer.SetCustomProperties(prop);
                gameObject.SetActive(false);
            }

        }
    }
}