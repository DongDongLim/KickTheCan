using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

namespace DH
{
    public class MapSetScript : MonoBehaviourPun
    {
        public int objIndex;
        Transform[] objectSpawnPos;
        ChanceAddon chanceAddon;
        private int randomResult;

        int randIndex;


        public void SetObjIndex(int index)
        {                
            photonView.RPC("ChildObjCreate", RpcTarget.AllBuffered, index);
            PhotonNetwork.Destroy(gameObject);              
        }

        [PunRPC]
        public void ChildObjCreate(int index)
        {
            objIndex = index;
            GameObject mapObject = Instantiate(MapSettingMng.instance.mapBG[objIndex], MapSettingMng.instance.gameObject.transform, false);
            mapObject.transform.position = transform.position;
            if(PhotonNetwork.IsMasterClient)
            {
                mapObject.GetComponent<MapSetting>().SetObjectSpawnPosList(ref objectSpawnPos);
                if (objectSpawnPos.Length == 0)
                    return;


                foreach (Transform obj in objectSpawnPos)
                {
                    Debug.Log("포이치지롱");
                    randomResult = chanceAddon.ChanceThree(10, 10, 80);
                    randIndex = Random.Range(0, MapSettingMng.instance.mapObj.Length);
                    switch (randomResult)
                    {
                        case 0:
                            Debug.Log("안생겼지롱");
                            break;
                        case 1:
                            PhotonNetwork.Instantiate("Obj", obj.position, obj.rotation, 0)
                            .GetComponent<ObjScript>().SetObjIndex(randIndex, MapSettingMng.instance.isRebuild);
                            Debug.Log("랜덤이지롱");
                            break;
                        case 2:
                            PhotonNetwork.Instantiate("Obj", obj.position, obj.rotation, 0)
                            .GetComponent<ObjScript>().SetObjIndex(Path.Combine("Sports", obj.name), MapSettingMng.instance.isRebuild);
                            Debug.Log("생겼지롱");
                            break;
                    }
                }
            }

        }
    }
}