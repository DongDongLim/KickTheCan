using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace DH
{
    public class CanSetScript : MonoBehaviourPun
    {
        [SerializeField]
        GameObject can;
        public void SetObjIndex(int index)
        {
            photonView.RPC("ChildObjCreate", RpcTarget.All, index);
        }

        [PunRPC]
        public void ChildObjCreate(int index)
        {
            GameObject canObj = Instantiate(can, MapSettingMng.instance.gameObject.transform, false);
            canObj.transform.position = transform.position;
            PlayMng.instance.can = canObj;
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