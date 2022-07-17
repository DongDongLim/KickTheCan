using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace DH
{
    public class CanMoveScript : MonoBehaviourPun
    {
        [SerializeField]
        float kickPower = 50;
        [SerializeField]
        Rigidbody rigid;
        [SerializeField]
        bool isMove;
        PhotonView view;

        private void OnEnable()
        {
            isMove = false;
            if (photonView.IsMine)
                view = GetComponent<PhotonView>();
        }


        public IEnumerator CanMove(Vector3 target, Player p)
        {
            if (!photonView.IsMine || isMove)
                yield break;
            isMove = true;
            PlayMng.instance.gameChat.SystemCanKickLog(p);
            rigid.AddForce(target * kickPower, ForceMode.Impulse);
            yield return null;
            while (rigid.velocity != Vector3.zero)
            {
                yield return null;
            }
            isMove = false;
            //photonView.RPC("SetLayer", RpcTarget.All, "Can");
            photonView.RPC("SetKinematic", RpcTarget.All);
        }

        [PunRPC]
        public void SetLayer(string name)
        {
            gameObject.layer = LayerMask.NameToLayer(name);
        }
        [PunRPC]
        public void SetKinematic()
        {
            rigid.isKinematic = true;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Tagger") && !isMove && !GameManager.Instance.isAttack && photonView.IsMine)
            {
                Debug.Log("캔참");
                int taggerId = collision.transform.gameObject.GetComponent<PlayerScript>().ownerID;
                foreach (Player p in PhotonNetwork.PlayerList)
                {
                    if (p.GetPlayerNumber() == taggerId)
                    {
                        Hashtable prob = new Hashtable { { GameData.PLAYER_TAGGER, true } };
                        p.SetCustomProperties(prob);
                    }
                }
                PhotonNetwork.Destroy(gameObject);
            }
        }

        private void OnApplicationQuit()
        {
            if (photonView.IsMine)
            {
                PhotonNetwork.RemoveRPCs(view);
                //PhotonNetwork.Destroy(view);
            }
        }

    }
}