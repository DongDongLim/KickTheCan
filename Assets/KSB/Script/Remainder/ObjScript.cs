using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace DH
{
    public class ObjScript : MonoBehaviourPun
    {
        public int objIndex;


        public void SetObjIndex(int index, bool isRebuild)
        {
            // TODO : 바뀜
            if (!isRebuild)
            {
                ChildObjCreate(index);
                return;
            }

            photonView.RPC("ChildObjCreate", RpcTarget.All, index);
        }

        [PunRPC]
        public void ChildObjCreate(int index)
        {
            objIndex = index;
            Instantiate(DH.MapSettingMng.instance.mapObj[objIndex], transform, false);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if(collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                GetComponent<Rigidbody>().isKinematic = true;
            }
        }
    }
}
