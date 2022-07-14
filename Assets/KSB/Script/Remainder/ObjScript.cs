using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

namespace DH
{
    public class ObjScript : MonoBehaviourPun
    {
        public int objIndex;
        PhotonView view;
        int cnt;

        private void OnEnable()
        {
            if (SceneManager.GetActiveScene().name == "LobbyScene")
                Destroy(gameObject);
        }

        public void SetObjIndex(int index)
        {
            GameObject obj = ChildObjCreate(index);
            cnt = obj.transform.childCount == 0 ? -1 : Random.Range(0, obj.transform.childCount);
            if(cnt != -1)
                obj.transform.GetChild(cnt).gameObject.SetActive(true);
            photonView.RPC("ChildObjCreate", RpcTarget.OthersBuffered, index, cnt);
            view = GetComponent<PhotonView>();
            Destroy(view);
        }

        public void SetObjIndex(string objName)
        {
            GameObject obj = ChildObjCreate(objName);
            cnt = obj.transform.childCount == 0 ? -1 : Random.Range(0, obj.transform.childCount);
            if (cnt != -1)
                obj.transform.GetChild(cnt).gameObject.SetActive(true);
            photonView.RPC("ChildObjCreate", RpcTarget.OthersBuffered, objName, cnt);
        }

        [PunRPC]
        public GameObject ChildObjCreate(int index, int childActiveCnt = -1)
        {
            objIndex = index;
            GameObject obj =
            Instantiate(MapSettingMng.instance.mapObj[objIndex], MapSettingMng.instance.gameObject.transform, false);
            obj.transform.position = transform.position;
            obj.transform.rotation = transform.rotation;
            if(childActiveCnt != -1)
            {
                obj.transform.GetChild(childActiveCnt).gameObject.SetActive(true);
            }
            gameObject.SetActive(false);
            return obj;
        }

        [PunRPC]
        public GameObject ChildObjCreate(string objName, int childActiveCnt = -1)
        {
            GameObject obj =
            (GameObject)Instantiate(Resources.Load(objName), MapSettingMng.instance.gameObject.transform, false);
            obj.transform.position = transform.position;
            obj.transform.rotation = transform.rotation;
            if (childActiveCnt != -1)
            {
                obj.transform.GetChild(childActiveCnt).gameObject.SetActive(true);
            }
            gameObject.SetActive(false);
            return obj;
        }

        private void OnDestroy()
        {
            if (photonView.IsMine)
                PhotonNetwork.RemoveRPCs(view);
            //PhotonNetwork.OpCleanRpcBuffer(GetComponent<PhotonView>());
        }

    }
}
