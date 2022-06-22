using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using System.IO;

namespace DH
{
    public class MapSettingMng : SingletonMini<MapSettingMng>
    {
        public GameObject[] mapBG;
        public GameObject[] mapObj;
        public GameObject taggerObj;

        private PlayerSceneInfo playerSceneInfo;

        public int objIndex;

        int randIndex;

        bool isRebuild = false;

        public GameObject[] objectSpawnPos;

        ChanceAddon chanceAddon;
        private int randomResult;

        private void Start()
        {
            playerSceneInfo = GameObject.FindGameObjectWithTag("DontDestroy").GetComponent<PlayerSceneInfo>();
            if (true == playerSceneInfo.isRenegade || true == playerSceneInfo.isObserver)
            {
                isRebuild = true;
            }
        }

        protected override void OnAwake()
        {
            chanceAddon = new ChanceAddon();
        }



    public IEnumerator Setting()
        {
            if (objectSpawnPos.Length == 0)
                yield return null;
            
            foreach(GameObject obj in objectSpawnPos)
            {
                Debug.Log("포이치지롱");
                randomResult = chanceAddon.ChanceThree(0,0,100);
                randIndex = Random.Range(0,mapObj.Length);
                switch(randomResult)
                {
                    case 0:
                        Debug.Log(obj.name);
                        Debug.Log("안생겼지롱");
                        break;
                    case 1:
                        Debug.Log(obj.name);
                        PhotonNetwork.Instantiate("Obj", obj.transform.position, Quaternion.identity, 0)
                        .GetComponent<ObjScript>().SetObjIndex(randIndex, isRebuild);
                        Debug.Log("랜덤이지롱");
                        break;
                    case 2:
                        Debug.Log(obj.name);
                        PhotonNetwork.Instantiate(Path.Combine("SportsMap", obj.name), obj.transform.position, Quaternion.identity, 0);
                        
                        Debug.Log("생겼지롱");
                        break;
                }
                yield return null;
            }
        }
        public void ChildObjCreate(int index)
        {
            objIndex = index;
            Instantiate(instance.mapObj[objIndex], transform, false);
        }

        public void TaggerSetting(Player p)
        {
            Debug.Log("술래 생성");
            GameObject playerObj = PhotonNetwork.Instantiate
                (DH.GameData.PLAYER_OBJECT, Vector3.up * 5, Quaternion.identity, 0);
            playerObj.AddComponent<TaggerController>();
            playerObj.GetComponent<TaggerSetScript>().SetObj(isRebuild);
            playerObj.GetComponent<PlayerScript>().ControllerSetting();
        }

        public void RunnerSetting(Player p)
        {
            Debug.Log("러너 생성");
            randIndex = Random.Range(0, mapObj.Length);
            GameObject playerObj = PhotonNetwork.Instantiate
                (DH.GameData.PLAYER_OBJECT, Vector3.up * 5, Quaternion.identity, 0);
            playerObj.AddComponent<RunnerController>();
            if (p == null)
                playerObj.GetComponent<RunnerSetScript>().SetObjIndex(randIndex, "Hide", isRebuild);
            else
                playerObj.GetComponent<RunnerSetScript>().SetObjIndex(randIndex, "Default", isRebuild);
            playerObj.GetComponent<PlayerScript>().ControllerSetting();
        }

        // TODO : 관전자 모드 
        public void ObserverSetting(Player p)
        {
            Debug.Log("관전자 모드");     
        }

    }
}
