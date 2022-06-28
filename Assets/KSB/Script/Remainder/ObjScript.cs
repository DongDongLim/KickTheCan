﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace DH
{
    public class ObjScript : MonoBehaviourPun
    {
        public int objIndex;


        public void SetObjIndex(int index)
        {         
            photonView.RPC("ChildObjCreate", RpcTarget.AllBuffered, index);
            PhotonNetwork.Destroy(gameObject);
        }

        public void SetObjIndex(string obj)
        {
            photonView.RPC("ChildObjCreate", RpcTarget.AllBuffered, obj);

            PhotonNetwork.Destroy(gameObject);
        }

        [PunRPC]
        public void ChildObjCreate(int index)
        {
            objIndex = index;
            Instantiate(MapSettingMng.instance.mapObj[objIndex], MapSettingMng.instance.gameObject.transform, false).transform.position = transform.position;
        }


        [PunRPC]
        public void ChildObjCreate(string obj)
        {
            GameObject gameObject =
            (GameObject)Instantiate(Resources.Load(obj), MapSettingMng.instance.gameObject.transform, false);
            gameObject.transform.position = transform.position;
            gameObject.transform.rotation = transform.rotation;
        }
    }
}
