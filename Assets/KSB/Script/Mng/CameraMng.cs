using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

namespace DH
{
    public class CameraMng : SingletonMini<CameraMng>
    {
        [SerializeField]
        Camera mainCam;
        [SerializeField]
        CinemachineVirtualCamera playerCam;
        [SerializeField]
        CinemachineVirtualCamera skyCam;

        GameObject playerObject;
        public GameObject observerObject;

        protected override void OnAwake()
        {
            playerCam.gameObject.SetActive(false);
        }

        public void PlayerCamSetting(GameObject player)
        {
            playerObject = player;
            playerCam.gameObject.SetActive(true);
            playerCam.Follow = playerObject.transform;
            playerCam.LookAt = playerObject.transform;
            skyCam.gameObject.SetActive(false);
        }

        public void SwitchCam()
        {
            if (playerCam.gameObject.activeSelf)
            {
                UIMng.instance.SetMoveUI(observerObject.GetComponent<PlayerMove>());
                observerObject.transform.position = playerObject.transform.position;
                skyCam.gameObject.SetActive(true);
                playerCam.gameObject.SetActive(false);
            }
            else
            {
                UIMng.instance.SetMoveUI(playerObject.GetComponent<PlayerMove>());
                playerCam.gameObject.SetActive(true);
                skyCam.gameObject.SetActive(false);
            }

        }

        public void TaggerCamSetting()
        {
            mainCam.cullingMask = ~(1 << LayerMask.NameToLayer("Hide"));
        }

        public void RunnerCamSetting()
        {
            // -1은 everyting
            mainCam.cullingMask = -1;
        }


    }
}
