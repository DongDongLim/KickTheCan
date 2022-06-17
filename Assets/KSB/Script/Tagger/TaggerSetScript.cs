using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace KSB
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
            Instantiate(KSB.MapSettingMng.instance.taggerObj, transform, false);
        }
    }
}
