using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;

using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace DH
{
    public class MapSettingMng : SingletonMini<MapSettingMng>
    {
        public GameObject[] mapObj;

        protected override void OnAwake()
        {

        }

        public IEnumerator Setting()
        {
            int randIndex;
            for (int i = 0; i < 100; ++i)
            {
                randIndex = Random.Range(0, mapObj.Length);
                PhotonNetwork.Instantiate
                    ("Obj", new Vector3(Random.Range(-25, 26), 10, Random.Range(-25, 26)), Quaternion.identity, 0)
                    .GetComponent<ObjScript>().SetObjIndex(randIndex);
                yield return null;
            }
        }

    }
}
