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
            PhotonNetwork.Destroy(gameObject);  
            // TODO : 포톤 뷰가 삭제 되었기 때문에 재입장 시에 다시 생성해주어야한다.
        }

        [PunRPC]
        public void ChildObjCreate(int index)
        {
            objIndex = index;
            Instantiate(MapSettingMng.instance.mapBG[objIndex], MapSettingMng.instance.gameObject.transform, false).transform.position = transform.position;
        }
    }
}