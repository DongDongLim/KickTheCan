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

        public void SetObjIndex(int index, string name , bool isRebuild)
        {
            UIMng.instance.testHideAction += ChangeLayer;
            if (isRebuild)
            {
                Debug.Log("TaggerSetScript");
                ChildObjCreate(index, name);
            }
            else
            {
                photonView.RPC("ChildObjCreate", RpcTarget.All, index, name);
            }
        }

        [PunRPC]
        public void ChildObjCreate(int index, string name)
        {
            change = new ChangeLayer();
            objIndex = index;
            Instantiate(DH.MapSettingMng.instance.mapObj[objIndex], transform, false);
            change.CangeTransformLayer(transform, name);
        }

        private void OnDestroy()
        {
            if(UIMng.instance != null)
                UIMng.instance.testHideAction -= ChangeLayer;
        }

        // TODO : Test
        public void ChangeLayer()
        {
            photonView.RPC("ChangeLayerFunc", RpcTarget.All);
        }

        [PunRPC]
        public void ChangeLayerFunc()
        {
            switch (LayerMask.LayerToName(gameObject.layer))
            {
                case "Hide":
                    change.CangeTransformLayer(transform, "Default");
                    break;
                case "Default":
                    change.CangeTransformLayer(transform, "Hide");
                    break;
            }
        }
    }
}
