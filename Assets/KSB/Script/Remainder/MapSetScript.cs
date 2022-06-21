using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace DH
{
    public class MapSetScript : MonoBehaviourPun
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
            Instantiate(MapSettingMng.instance.mapBG[objIndex], MapSettingMng.instance.gameObject.transform, false);
        }
    }
}