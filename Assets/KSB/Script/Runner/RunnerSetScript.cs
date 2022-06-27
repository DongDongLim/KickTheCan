using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace DH
{
    public class RunnerSetScript : MonoBehaviourPun
    {
        public int objIndex;
        ChangeLayer change;

        public void SetObjIndex(int index, string name)
        {     
            photonView.RPC("ChildObjCreate", RpcTarget.All, index, name);
        }

        [PunRPC]
        public void ChildObjCreate(int index, string name)
        {
            change = new ChangeLayer();
            if (index != -1)
            {
                objIndex = index;
                Instantiate(DH.MapSettingMng.instance.mapObj[objIndex], transform, false);
            }
            change.CangeTransformLayer(transform, name, true);
        }
    }
}
