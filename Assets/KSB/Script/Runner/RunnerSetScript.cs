using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace KSB
{
    public class RunnerSetScript : MonoBehaviourPun
    {
        public int objIndex;

        public void SetObjIndex(int index)
        {
            photonView.RPC("ChildObjCreate", RpcTarget.All, index);
        }

        [PunRPC]
        public void ChildObjCreate(int index)
        {
            objIndex = index;
            Instantiate(KSB.MapSettingMng.instance.mapObj[objIndex], transform, false);
        }
    }
}
