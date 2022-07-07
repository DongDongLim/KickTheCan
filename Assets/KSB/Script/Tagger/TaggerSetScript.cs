using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace DH
{
    public class TaggerSetScript : MonoBehaviourPun
    {
        public void SetObj()
        {
            photonView.RPC("ChildObjCreate", RpcTarget.All);
        }

        [PunRPC]
        public void ChildObjCreate()
        {
            Instantiate(MapSettingMng.instance.taggerObj, transform, false);
        }
    }
}
