using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace DH
{
    public class RunnerSetScript : MonoBehaviourPun
    {
        public int objIndex;

        public void SetObjIndex(int index, bool isRebuild)
        {
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
            foreach(Transform child in transform)
            {
                child.gameObject.layer = gameObject.layer;
            }
        }
    }
}
