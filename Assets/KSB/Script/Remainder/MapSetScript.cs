using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace DH
{
    public class MapSetScript : MonoBehaviourPun
    {
        public int objIndex;
        ChanceAddon chanceAddon;
        private int randomResult;
        GameObject mapObject;

        int randIndex;


        public void SetObjIndex(int index)
        {
            chanceAddon = new ChanceAddon();
            GameManager.Instance.canCheckActionTrue += CanSpawn;
            photonView.RPC("ChildObjCreate", RpcTarget.AllBuffered, index);
        }

        [PunRPC]
        public void ChildObjCreate(int index)
        {
            objIndex = index;
            mapObject = Instantiate(MapSettingMng.instance.mapBG[objIndex], MapSettingMng.instance.gameObject.transform, false);
            mapObject.transform.position = transform.position;
            MapSettingMng.instance.curMap = mapObject;
            ref Transform[] objectSpawnPos = ref MapSettingMng.instance.objectSpawnPos;
            mapObject.GetComponent<MapSetting>().SetObjectSpawnPosList(ref objectSpawnPos);
            if (PhotonNetwork.IsMasterClient)
            {
                if (objectSpawnPos.Length == 0)
                    return;


                foreach (Transform obj in objectSpawnPos)
                {
                    randomResult = chanceAddon.ChanceThree(10, 10, 80);
                    randIndex = Random.Range(0, MapSettingMng.instance.mapObj.Length);
                    switch (randomResult)
                    {
                        case 0:
                            break;
                        case 1:
                            PhotonNetwork.Instantiate("Obj", obj.position, obj.rotation, 0)
                            .GetComponent<ObjScript>().SetObjIndex(randIndex);
                            break;
                        case 2:
                            PhotonNetwork.Instantiate("Obj", obj.position, obj.rotation, 0)
                            .GetComponent<ObjScript>().SetObjIndex(Path.Combine("Sports", obj.name));
                            break;
                    }
                }
                CanSpawn();
            }
            gameObject.SetActive(false);

        }

        public void CanSpawn()
        {
            if (PhotonNetwork.LocalPlayer == PhotonNetwork.MasterClient)
            {
                GameObject obj =
                PhotonNetwork.Instantiate
                   ("Can", mapObject.GetComponent<MapSetting>().CanSpqwnPos(), Quaternion.identity, 0);
                Debug.Log("obj" + obj.transform.position);
            }
        }

        private void OnDestroy()
        {
            if (photonView.IsMine)
                PhotonNetwork.OpCleanRpcBuffer(GetComponent<PhotonView>());
        }
    }
}