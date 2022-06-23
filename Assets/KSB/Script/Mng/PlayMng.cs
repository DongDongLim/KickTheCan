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

        public YSM.GameChat gameChat;

        public GameObject can;


        protected override void OnAwake()
        {

        }

        public void BeCaught(GameObject player)
        {
            PhotonNetwork.Destroy(player);
            isRunnerBeCaught = true;
            UIMng.instance.jumpAction += TestRelease;
            gameChat.SetCharacterType(YSM.GameCharacterType.DEAD);

        }

        public void Release()
        {
            if (isRunnerBeCaught)
            {
                MapSettingMng.instance.RunnerSetting(null);
                UIMng.instance.jumpAction -= Release;
                isRunnerBeCaught = false;
                gameChat.SetCharacterType(YSM.GameCharacterType.RUNNER);
            }
        }

        public void TestRelease()
        {
            //photonView.RPC("KickTheCan", RpcTarget.All, Vector3.zero);
        }

        public void KickTheCan(Vector3 canTargetVector)
        {
            StartCoroutine(can?.GetComponent<CanMoveScript>().CanMove(canTargetVector));
            Release();
        }
    }
}