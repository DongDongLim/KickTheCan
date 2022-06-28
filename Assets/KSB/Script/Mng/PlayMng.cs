using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

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
            UIDataMng.Instance.SetRunner(UIDataMng.Instance.RUNNER_LIFE - 1);
            PhotonNetwork.Destroy(player);
            isRunnerBeCaught = true;
            UIMng.instance.jumpAction += Release;
            PlayMng.instance.gameChat.SetCharacterType(YSM.GameCharacterType.DEAD);

        }

        public void Release()
        {
            if (isRunnerBeCaught)
            {
                MapSettingMng.instance.RunnerSetting("Hide");
                isRunnerBeCaught = false;
                PlayMng.instance.gameChat.SetCharacterType(YSM.GameCharacterType.RUNNER);
            }
        }

        public void KickTheCan(Vector3 canTargetVector, Player p)
        {
            StartCoroutine(can?.GetComponent<CanMoveScript>().CanMove(canTargetVector, p));
        }
    }
}