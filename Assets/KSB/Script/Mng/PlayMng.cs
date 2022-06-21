using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace DH
{
    public class PlayMng : SingletonMini<PlayMng>
    {
        [SerializeField]
        bool isRunnerBeCaught = false;

        public GameObject can;

        protected override void OnAwake()
        {

        }

        public void BeCaught(GameObject player)
        {
            PhotonNetwork.Destroy(player);
            isRunnerBeCaught = true;
            UIMng.instance.jumpAction += Release;
        }

        public void Release()
        {
            if (isRunnerBeCaught)
            {
                MapSettingMng.instance.RunnerSetting(null);
                UIMng.instance.jumpAction -= Release;
                isRunnerBeCaught = false;
            }
        }

        [PunRPC]
        public void KickTheCan(Vector3 canTargetVector)
        {
            StartCoroutine(can.GetComponent<CanMoveScript>().CanMove(canTargetVector));
        }

        [PunRPC]
        public void SetCanPosition(Vector3 pos)
        {
            can.transform.position = pos;
        }
    }
}