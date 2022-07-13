using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace DH
{
    public class TaggerSetScript : MonoBehaviourPun
    {
        ChangeLayer change;
        public void SetObj()
        {
            photonView.RPC("ChildObjCreate", RpcTarget.AllBuffered);
        }

        [PunRPC]
        public void ChildObjCreate()
        {
            Instantiate(MapSettingMng.instance.taggerObj, transform, false);
            change = new ChangeLayer();
            change.CangeTransformLayer(transform, "Tagger", false);
        }
    }
}
