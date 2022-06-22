using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace DH
{
    public class TaggerSetScript : MonoBehaviourPun
    {
        public void SetObj(bool isRebuild)
        {
            if (isRebuild)
            {
                Debug.Log("TaggerSetScript");
                ChildObjCreate();                
            }
            else
            {
                photonView.RPC("ChildObjCreate", RpcTarget.All);
            }            
        }

        [PunRPC]
        public void ChildObjCreate()
        {
            Instantiate(DH.MapSettingMng.instance.taggerObj, transform, false);
        }
    }
}
