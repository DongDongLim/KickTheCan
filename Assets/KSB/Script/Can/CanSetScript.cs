using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace DH
{
    public class CanSetScript : MonoBehaviourPun
    {
        [SerializeField]
        GameObject can;

        public void SetObjIndex()
        { 
            photonView.RPC("ChildObjCreate", RpcTarget.AllBuffered);
        }

        [PunRPC]
        public void ChildObjCreate()
        {
            GameObject canObj = Instantiate(can, MapSettingMng.instance.gameObject.transform, false);
            canObj.transform.position = transform.position;
            PlayMng.instance.can = canObj;
        }
    }
}