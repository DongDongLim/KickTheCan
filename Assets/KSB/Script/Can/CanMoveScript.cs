using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace DH
{
    public class CanMoveScript : MonoBehaviourPun
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
            if (rigid.velocity != Vector3.zero || !photonView.IsMine)
                yield break;
            rigid.AddForce(target * kickPower, ForceMode.Impulse);
            while(rigid.velocity != Vector3.zero)
            {

            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Tagger"))
            {
                //PlayMng.instance.photonView.RPC("KickTheCan", RpcTarget.All, Vector3.Normalize(collision.gameObject.transform.position - transform.position));

            }
        }

    }
}