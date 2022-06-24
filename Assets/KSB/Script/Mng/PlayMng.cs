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
            GameManager.Instance.canCheckActionFalse += Release;
        }

        public void BeCaught(GameObject player)
        {
            PhotonNetwork.Destroy(player);
            isRunnerBeCaught = true;
            gameChat.SetCharacterType(YSM.GameCharacterType.DEAD);
        }

        public void Release()
        {
            if (isRunnerBeCaught)
            {
                MapSettingMng.instance.RunnerSetting(null);
                isRunnerBeCaught = false;
                gameChat.SetCharacterType(YSM.GameCharacterType.RUNNER);
            }
        }


        public void KickTheCan(Vector3 canTargetVector, Player p)
        {
            StartCoroutine(can?.GetComponent<CanMoveScript>().CanMove(canTargetVector, p));
        }
    }
}