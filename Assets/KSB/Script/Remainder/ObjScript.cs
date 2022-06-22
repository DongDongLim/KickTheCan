using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace DH
{
    public class ObjScript : MonoBehaviourPun
    {
        public int objIndex;


        public void SetObjIndex(int index, bool isRebuild)
        {
            // TODO : 바뀜
            if (!isRebuild)
            {
                ChildObjCreate(index);
                return;
            }

            photonView.RPC("ChildObjCreate", RpcTarget.All, index);

            PhotonNetwork.Destroy(gameObject);
        }

        [PunRPC]
        public void ChildObjCreate(int index)
        {
            objIndex = index;
            Instantiate(MapSettingMng.instance.mapObj[objIndex], MapSettingMng.instance.gameObject.transform, false).transform.position = transform.position;
        }
    }
}
