using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.IO;
using Photon.Pun.UtilityScripts;

namespace DH
{
    public class MapSettingMng : SingletonMini<MapSettingMng>
    {
        public Vector3 taggerSpawnPos;
        public Vector3 runnerSpawnPos;

        public GameObject[] mapBG;
        public GameObject[] mapObj;
        public GameObject taggerObj;       

        public GameObject curMap;
        public Transform[] objectSpawnPos;

        public Vector3 canTransform;

        public int objIndex;

        public List<GameObject> playerObjList;

        int randIndex;


        protected override void OnAwake()
        {
        }

        public IEnumerator Setting()
        {
            randIndex = Random.Range(0, mapBG.Length);
            PhotonNetwork.Instantiate
                    ("Map", Vector3.zero, Quaternion.identity, 0)
                    .GetComponent<MapSetScript>().SetObjIndex(randIndex);
            yield return null;
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
                (DH.GameData.PLAYER_OBJECT, taggerSpawnPos + Vector3.up, Quaternion.identity, 0);
            playerObj.AddComponent<TaggerController>();
            playerObj.GetComponent<TaggerSetScript>().SetObj();
            playerObj.GetComponent<PlayerScript>().ControllerSetting();
            PlayMng.instance.gameChat.SetCharacterType(YSM.GameCharacterType.TAGGER);

            UIDataMng.Instance.SetTagger(UIDataMng.Instance.TAGGER_LIFE + 1);
            UISetting(true);

            // TODO : (Test) GameOver / SH          
            playerObjList.Add(playerObj);
        }

        public void RunnerSetting(string layerName)
        {
            Debug.Log("러너 생성");
            randIndex = Random.Range(0, 11);//mapObj.Length + objectSpawnPos.Length);
            GameObject playerObj = PhotonNetwork.Instantiate
                (GameData.PLAYER_OBJECT, runnerSpawnPos + Vector3.up * 3, Quaternion.identity, 0);
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
            UISetting(true);
            
            // TODO : (Test) GameOver / SH         
            playerObjList.Add(playerObj);
        }

        public void ObserverSetting(Player p)
        {
            Debug.Log("관전자 모드");
            CameraMng.instance.SwitchCam();
            PlayMng.instance.gameChat.SetCharacterType(YSM.GameCharacterType.OBSERVER);
        }

        public void UISetting(bool isTagger)
        {
            DH.UIMng.instance.loadingScreen.SetActive(false);
            DH.UIMng.instance.taggerCaption.SetActive(isTagger);
            DH.UIMng.instance.runnerCaption.SetActive(!isTagger);
            DH.UIMng.instance.startCount.SetActive(true);
        }

    }
}
