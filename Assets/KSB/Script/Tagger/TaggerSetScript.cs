﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace DH
{
    public class TaggerSetScript : MonoBehaviourPun
    {
        public void SetObj()
        {          
            photonView.RPC("ChildObjCreate", RpcTarget.AllBuffered);
        }

        [PunRPC]
        public void ChildObjCreate()
        {
            Instantiate(DH.MapSettingMng.instance.taggerObj, transform, false);
        }
    }
}
