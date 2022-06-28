using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace DH
{
    public class TaggerSetScript : MonoBehaviourPun
    {
        ChangeLayer change;
        public void SetObj(string name)
        {
            photonView.RPC("ChildObjCreate", RpcTarget.AllBuffered, name);
        }

        [PunRPC]
        public void ChildObjCreate(string name)
        {
            change = new ChangeLayer();
            Instantiate(DH.MapSettingMng.instance.taggerObj, transform, false);
            change.CangeTransformLayer(transform, name, false);
        }
    }
}
