using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.IO;
using Photon.Pun.UtilityScripts;
using System.IO;

namespace DH
{
    public class MapSettingMng : SingletonMini<MapSettingMng>
    {
        public GameObject[] mapBG;
        public GameObject[] mapObj;
        public GameObject taggerObj;
        public GameObject curMap;
        public Transform[] objectSpawnPos;

        private PlayerSceneInfo playerSceneInfo;

        public int objIndex;

        int randIndex;   

        public GameObject[] objectSpawnPos;

        ChanceAddon chanceAddon;
        private int randomResult;

        private void Start()
        {
            playerSceneInfo = GameObject.FindGameObjectWithTag("DontDestroy").GetComponent<PlayerSceneInfo>();        
        }

        protected override void OnAwake()
        {
            chanceAddon = new ChanceAddon();
        }        

    public IEnumerator Setting()
        {
            randIndex = Random.Range(0, mapBG.Length);
            PhotonNetwork.Instantiate
                    ("Map", Vector3.zero, Quaternion.identity, 0)
                    .GetComponent<MapSetScript>().SetObjIndex(randIndex);
            yield return null;
            canTransform = new Vector3(15, 0.5f, 10);
            PhotonNetwork.Instantiate
                   ("Can", canTransform, Quaternion.identity, 0).GetComponent<CanSetScript>().SetObjIndex();
        }

            if (objectSpawnPos.Length == 0)
                yield break;

            foreach (GameObject obj in objectSpawnPos)
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
                        .GetComponent<ObjScript>().SetObjIndex(randIndex);
                        Debug.Log("랜덤이지롱");
                        break;
                    case 2:
                        Debug.Log(obj.name);
                        PhotonNetwork.Instantiate(Path.Combine("Sports", obj.name), obj.transform.position, Quaternion.identity, 0);
                        
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
            playerObj.GetComponent<TaggerSetScript>().SetObj("Tagger");
            playerObj.GetComponent<PlayerScript>().ControllerSetting();
            PlayMng.instance.gameChat.SetCharacterType(YSM.GameCharacterType.TAGGER);

            UIDataMng.Instance.SetTagger(UIDataMng.Instance.TAGGER_LIFE + 1);
        }

        public void RunnerSetting(string layerName)
        {
            Debug.Log("러너 생성");
            randIndex = Random.Range(0, mapObj.Length + objectSpawnPos.Length);
            GameObject playerObj = PhotonNetwork.Instantiate
                (GameData.PLAYER_OBJECT, Vector3.up * 5, Quaternion.identity, 0);
            playerObj.AddComponent<RunnerController>();
            if (randIndex < mapObj.Length)
            {
                playerObj.GetComponent<RunnerSetScript>().SetObjIndex(randIndex, layerName);
            }
            else
            {
                Debug.Log("생성");
                playerObj.GetComponent<RunnerSetScript>().SetObjIndex(randIndex,
                    Path.Combine("Sports", objectSpawnPos[randIndex - mapObj.Length].name), layerName);
            }
            playerObj.GetComponent<PlayerScript>().ControllerSetting();
            PlayMng.instance.gameChat.SetCharacterType(YSM.GameCharacterType.RUNNER);
        }
               
        public void ObserverSetting(Player p)
        {
            Debug.Log("관전자 모드");                       
            CameraMng.instance.SwitchCam();
            PlayMng.instance.gameChat.SetCharacterType(YSM.GameCharacterType.OBSERVER);
        }

    }
}
