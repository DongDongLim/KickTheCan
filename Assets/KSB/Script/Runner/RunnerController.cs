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
        ChangeLayer change;

        private void Awake()
        {
            CameraMng.instance.RunnerCamSetting();
            GameManager.Instance.canCheckActionTrue += ChangeLayer;
        }

        private void OnDisable()
        {
            if (GameManager.Instance != null)
                GameManager.Instance.canCheckActionTrue -= ChangeLayer;
        }

        public void ChangeLayer()
        {
            GetComponent<RunnerSetScript>().photonView.RPC("ChildObjCreate", RpcTarget.All, -1, "Default");
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
            if(other.gameObject.layer == LayerMask.NameToLayer("Weapon"))
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
                collision.gameObject.layer = LayerMask.NameToLayer("Default");
                Hashtable hashtable = new Hashtable { {GameData.PLAYER_ISKICK, true } };
                PhotonNetwork.LocalPlayer.SetCustomProperties(hashtable);
                owner.photonView.RPC("KickTheCan", RpcTarget.MasterClient, Vector3.Normalize(collision.gameObject.transform.position - transform.position), PhotonNetwork.LocalPlayer);               
            }
        }

    }
}