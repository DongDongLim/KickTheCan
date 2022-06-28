using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace DH
{
    public class CanMoveScript : MonoBehaviour
    {
        [SerializeField]
        float kickPower = 10;

        Rigidbody rigid;

        private void Start()
        {
            rigid = GetComponent<Rigidbody>();
        }


        public IEnumerator CanMove(Vector3 target)
        {
            if (rigid.velocity != Vector3.zero)
                yield break;
            rigid.AddForce(target * kickPower, ForceMode.Impulse);
            while(rigid.velocity != Vector3.zero)
            {

            }
            if (PhotonNetwork.IsMasterClient)
                PlayMng.instance.photonView.RPC("SetCanPosition", RpcTarget.Others, transform.position);
        }

    }
}