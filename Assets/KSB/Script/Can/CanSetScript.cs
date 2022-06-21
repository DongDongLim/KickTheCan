using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace DH
{
    public class CanSetScript : MonoBehaviourPun
    {
        [SerializeField]
        GameObject Can;
        public void SetObjIndex(int index)
        {
            photonView.RPC("ChildObjCreate", RpcTarget.All, index);
        }

        [PunRPC]
        public void ChildObjCreate(int index)
        {
            Instantiate(Can, MapSettingMng.instance.gameObject.transform, false).transform.position = transform.position;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                GetComponent<Rigidbody>().isKinematic = true;
            }
        }
    }
}