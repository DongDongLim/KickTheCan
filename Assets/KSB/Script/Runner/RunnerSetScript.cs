using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace DH
{
    public class RunnerSetScript : MonoBehaviourPun
    {
        public int objIndex;
        ChangeLayer change;

        public void SetObjIndex(int index, string name)
        {
            photonView.RPC("ChildObjCreate", RpcTarget.AllBuffered, index, name);
        }

        public void SetObjIndex(int index, string objName, string name)
        {
            photonView.RPC("ChildObjCreate", RpcTarget.AllBuffered, index, objName, name);
        }


        [PunRPC]
        public void ChildObjCreate(int index, string name)
        {
            change = new ChangeLayer();
            if (index != -1)
            {
                objIndex = index;
                Instantiate(MapSettingMng.instance.mapObj[objIndex], transform, false);
            }
            change.CangeTransformLayer(transform, name, true);
        }

        [PunRPC]
        public void ChildObjCreate(int index, string objName, string name)
        {
            if (index != -1)
            {
                objIndex = index;
                Debug.Log("진짜생성");
                Instantiate(Resources.Load(objName), transform, false);
            }
            change = new ChangeLayer();
            change.CangeTransformLayer(transform, name, true);
        }
    }
}
