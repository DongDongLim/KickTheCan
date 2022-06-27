using System.Collections;
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
            photonView.RPC("ChildObjCreate", RpcTarget.All, index);
            PhotonNetwork.Destroy(gameObject);
        }

        public void SetObjIndex(string obj, bool isRebuild)
        {
            // TODO : 바뀜
            if (isRebuild)
            {
                ChildObjCreate(obj);
                Destroy(gameObject);
                return;
            }

            photonView.RPC("ChildObjCreate", RpcTarget.All, obj);

            PhotonNetwork.Destroy(gameObject);
        }

        [PunRPC]
        public void ChildObjCreate(int index)
        {
            objIndex = index;
            GameObject gameObject =
            Instantiate(MapSettingMng.instance.mapObj[objIndex], MapSettingMng.instance.gameObject.transform, false);
            gameObject.transform.position = transform.position;
            gameObject.transform.rotation = transform.rotation;
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
