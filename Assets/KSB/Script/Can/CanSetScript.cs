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
        public void SetObjIndex(bool isRebuild)
        {
            if (isRebuild)
            {
                ChildObjCreate();
                return;
            }

            photonView.RPC("ChildObjCreate", RpcTarget.All);
        }

        [PunRPC]
        public void ChildObjCreate()
        {
            GameObject canObj = Instantiate(can, transform, false);
            PlayMng.instance.can = gameObject;
        }
    }
}