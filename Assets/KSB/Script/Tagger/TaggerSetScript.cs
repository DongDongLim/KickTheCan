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
                Destroy(gameObject);
                Debug.Log("TaggerSetScript : 재입장");
                ChildObjCreate();
            }           

            photonView.RPC("ChildObjCreate", RpcTarget.AllBuffered);
        }

        [PunRPC]
        public void ChildObjCreate()
        {
            Instantiate(DH.MapSettingMng.instance.taggerObj, transform, false);
        }
    }
}
