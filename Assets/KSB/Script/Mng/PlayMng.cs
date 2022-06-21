using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;
namespace DH
{
    public class PlayMng : SingletonMini<PlayMng>
    {
        [SerializeField]
        bool isRunnerBeCaught = false;


        protected override void OnAwake()
        {

        }

        public void BeCaught(GameObject player)
        {
            Destroy(player);
            isRunnerBeCaught = true;
            UIMng.instance.jumpAction += Release;
            UIMng.instance.jumpAction += Release;
        }

        public void Release()
        {
            if (isRunnerBeCaught)
                MapSettingMng.instance.RunnerSetting(null);
        }
    }
}