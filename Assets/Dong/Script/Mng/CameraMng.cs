using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

namespace DH
{
    public class CameraMng : SingletonMini<CameraMng>
    {
        [SerializeField]
        CinemachineVirtualCamera playerCam;
        [SerializeField]
        CinemachineVirtualCamera skyCam;

        protected override void OnAwake()
        {
            playerCam.gameObject.SetActive(false);
        }

        public void PlayerCamSetting(Transform transform)
        {
            playerCam.gameObject.SetActive(true);
            playerCam.Follow = transform;
            playerCam.LookAt = transform;
            skyCam.gameObject.SetActive(false);
        }
    }
}
