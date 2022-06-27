﻿using System.Collections;
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

        public Vector3 canTransform;

        public int objIndex;
        public bool isRebuild = false;


        private PlayerSceneInfo playerSceneInfo;


        int randIndex;

        public GameObject testCan;



        private void Start()
        {
            playerSceneInfo = GameObject.FindGameObjectWithTag("DontDestroy").GetComponent<PlayerSceneInfo>();        
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
            yield return null;
            canTransform = new Vector3(15, 0.5f, 10);
            PhotonNetwork.Instantiate
                   ("Can", canTransform, Quaternion.identity, 0).GetComponent<CanSetScript>().SetObjIndex(isRebuild);
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
            playerObj.GetComponent<TaggerSetScript>().SetObj("Tagger", isRebuild);
            playerObj.GetComponent<PlayerScript>().ControllerSetting();
            PlayMng.instance.gameChat.SetCharacterType(YSM.GameCharacterType.TAGGER);
        }

        public void RunnerSetting(Player p)
        {
            
            Debug.Log("러너 생성");
            randIndex = Random.Range(0, mapObj.Length);
            GameObject playerObj = PhotonNetwork.Instantiate
                (DH.GameData.PLAYER_OBJECT, Vector3.up * 5, Quaternion.identity, 0);
            playerObj.AddComponent<RunnerController>();
            if (p == null)
                playerObj.GetComponent<RunnerSetScript>().SetObjIndex(randIndex, "Hide");
            else
                playerObj.GetComponent<RunnerSetScript>().SetObjIndex(randIndex, "Default");
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
