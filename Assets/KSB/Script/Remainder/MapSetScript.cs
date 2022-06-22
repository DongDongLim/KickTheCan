using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace DH
{
    public class MapSetScript : MonoBehaviourPun
    {
        public int objIndex;


        public void SetObjIndex(int index, bool isRebuild)
        {
            if (isRebuild)
            {
                ChildObjCreate(index);
                Destroy(gameObject);
                return;
            }
            photonView.RPC("ChildObjCreate", RpcTarget.All, index);
            PhotonNetwork.Destroy(gameObject);
        }

        [PunRPC]
        public void ChildObjCreate(int index)
        {
            objIndex = index;
            Instantiate(MapSettingMng.instance.mapBG[objIndex], MapSettingMng.instance.gameObject.transform, false).transform.position = transform.position;
        }
    }
}