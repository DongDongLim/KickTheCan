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
        bool isFreeze = false;
    
        private void Awake()
        {
            CameraMng.instance.RunnerCamSetting();
            GameManager.Instance.canCheckActionTrue += ChangeLayer;

            UIMng.instance.SetUI("Runner");
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
            owner?.DieAnim();
        }

        void Freeze()
        {
            isFreeze = !isFreeze;
            owner.photonView.RPC("FreezeRigid", RpcTarget.All);
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.gameObject.layer == LayerMask.NameToLayer("Weapon"))
            {   
                // 추가
                // TODO : (Test) GameOver    
                //Hashtable props = new Hashtable() { { global::GameData.PLAYER_DEAD, true } };
                //PhotonNetwork.LocalPlayer.SetCustomProperties(props);
                // =================================

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
                Hashtable hashtable = new Hashtable { { GameData.PLAYER_ISKICK, true } };
                PhotonNetwork.LocalPlayer.SetCustomProperties(hashtable);
                owner.photonView.RPC("KickTheCan", RpcTarget.MasterClient, Vector3.Normalize(collision.gameObject.transform.position - transform.position), PhotonNetwork.LocalPlayer);               

            }
        }

    }
}