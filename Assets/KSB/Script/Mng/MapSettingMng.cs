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
        public GameObject[] mapObj;
        public GameObject taggerObj;
        int randIndex;

        protected override void OnAwake()
        {

        }

        public IEnumerator Setting()
        {
            for (int i = 0; i < 100; ++i)
            {
                randIndex = Random.Range(0, mapObj.Length);
                PhotonNetwork.Instantiate
                    ("Obj", new Vector3(Random.Range(-25, 26), 10, Random.Range(-25, 26)), Quaternion.identity, 0)
                    .GetComponent<ObjScript>().SetObjIndex(randIndex);
                yield return null;
            }
        }

        public void TaggerSetting(Player p)
        {
            Debug.Log("술래 생성");
            GameObject playerObj = PhotonNetwork.Instantiate
                (DH.GameData.PLAYER_OBJECT, Vector3.up * 5, Quaternion.identity, 0);
            playerObj.AddComponent<TaggerController>();
            playerObj.GetComponent<TaggerSetScript>().SetObj();
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
                playerObj.GetComponent<RunnerSetScript>().SetObjIndex(randIndex, "Hide");
            else
                playerObj.GetComponent<RunnerSetScript>().SetObjIndex(randIndex, "Default");
            playerObj.GetComponent<PlayerScript>().ControllerSetting();
        }

    }
}
