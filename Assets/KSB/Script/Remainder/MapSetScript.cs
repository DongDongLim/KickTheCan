﻿using System.Collections;
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

        int randIndex;


        public void SetObjIndex(int index)
        {
            chanceAddon = new ChanceAddon();
            photonView.RPC("ChildObjCreate", RpcTarget.AllBuffered, index);
            PhotonNetwork.Destroy(gameObject);  
            // TODO : 포톤 뷰가 삭제 되었기 때문에 재입장 시에 다시 생성해주어야한다.
        }

        [PunRPC]
        public void ChildObjCreate(int index)
        {
            objIndex = index;
            GameObject mapObject = Instantiate(MapSettingMng.instance.mapBG[objIndex], MapSettingMng.instance.gameObject.transform, false);
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
                            .GetComponent<ObjScript>().SetObjIndex(randIndex);
                            Debug.Log("랜덤이지롱");
                            break;
                        case 2:
                            PhotonNetwork.Instantiate("Obj", obj.position, obj.rotation, 0)
                            .GetComponent<ObjScript>().SetObjIndex(Path.Combine("Sports", obj.name));
                            Debug.Log("생겼지롱");
                            break;
                    }
                }
            }

        }
    }
}