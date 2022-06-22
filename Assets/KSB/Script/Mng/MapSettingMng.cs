using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;

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

        }

        public IEnumerator Setting()
        {
            randIndex = Random.Range(0, mapBG.Length);
            PhotonNetwork.Instantiate
                    ("Map", Vector3.zero, Quaternion.identity, 0)
                    .GetComponent<MapSetScript>().SetObjIndex(randIndex, isRebuild);

            // TODO : Test 중 / 로비에서 재접속 시 맵만 생성됨
            if (isRebuild)
            {
                PhotonNetwork.Instantiate
                        ("Obj", new Vector3(Random.Range(-25, 26), 10, Random.Range(-25, 26)), Quaternion.identity, 0)
                        .GetComponent<ObjScript>().SetObjIndex(randIndex, isRebuild);
            }
            else
            {
                for (int i = 0; i < 100; ++i)
                {
                    randIndex = Random.Range(0, mapObj.Length);

                    PhotonNetwork.Instantiate
                        ("Obj", new Vector3(Random.Range(-25, 26), 10, Random.Range(-25, 26)), Quaternion.identity, 0)
                        .GetComponent<ObjScript>().SetObjIndex(randIndex, isRebuild);
                    yield return null;
                }
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
