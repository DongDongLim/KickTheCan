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
        bool isMove;

        private void Start()
        {
            isMove = false;
        }


        public IEnumerator CanMove(Vector3 target, Player p)
        {
            if (!photonView.IsMine || isMove)
                yield break;
            isMove = true;
            PlayMng.instance.gameChat.SystemCanKickLog(p);
            rigid.AddForce(target * kickPower, ForceMode.Impulse);
            while (rigid.velocity != Vector3.zero)
            {
                yield return null;
            }
            isMove = false;
            photonView.RPC("SetLayer", RpcTarget.All, "Can");
        }

        [PunRPC]
        public void SetLayer(string name)
        {
            gameObject.layer = LayerMask.NameToLayer(name);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Tagger") && !isMove && !GameManager.Instance.isAttack && photonView.IsMine)
            {
                Debug.Log("먹음");
                isMove = true;
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

    }
}