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
                Destroy(gameObject);
                Debug.Log("TaggerSetScript : 재입장");
                ChildObjCreate(index);
            }           

            photonView.RPC("ChildObjCreate", RpcTarget.AllBuffered, index);
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