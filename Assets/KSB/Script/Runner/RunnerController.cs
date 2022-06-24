using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace DH
{
    public class RunnerController : Controller, IDamaged
    {
        private void Awake()
        {
            CameraMng.instance.RunnerCamSetting();
        }

        public override void ControllerAction()
        {
            Freeze();
        }

        public override void ControlUpdate()
        {
            move.GroundChecker();
        }

        public void Damaged()
        {
            CameraMng.instance.SwitchCam();
            PlayMng.instance.BeCaught(gameObject);
        }

        void Freeze()
        {
            owner.photonView.RPC("FreezeRigid", RpcTarget.All);
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.gameObject.layer == LayerMask.NameToLayer("Weapon") && gameObject.layer == LayerMask.NameToLayer("Default"))
            {
                Damaged();
                int id = other.transform.parent.parent.GetComponent<PlayerScript>().ownerID;
                foreach(Player p in PhotonNetwork.PlayerList)
                {
                    if (id == p.GetPlayerNumber())
                    {
                        PlayMng.instance.gameChat.SystemKillLog(p, PhotonNetwork.LocalPlayer);
                        break;
                    }
                }

            }
        }

        

        private void OnCollisionEnter(Collision collision)
        {
            if(collision.gameObject.layer == LayerMask.NameToLayer("Can"))
            {
                owner.photonView.RPC("KickTheCan", RpcTarget.All, Vector3.Normalize(collision.gameObject.transform.position - transform.position));
                Hashtable hashtable = new Hashtable { {GameData.PLAYER_ISKICK, true } };
                PhotonNetwork.LocalPlayer.SetCustomProperties(hashtable);
                PlayMng.instance.gameChat.AddCanKickLog(PhotonNetwork.LocalPlayer);
            }
        }

    }
}