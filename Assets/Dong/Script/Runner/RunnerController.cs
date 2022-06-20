﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace DH
{
    public class RunnerController : Controller, IDamaged
    {
        bool isFreeze = false;

        private void Awake()
        {
            CameraMng.instance.RunnerCamSetting();
        }

        public override void ControllerAction()
        {
            Freeze();
        }

        public override void ControlUpdate()
        {
            move.GroundChecker();
        }

        public void Damaged()
        {
            CameraMng.instance.SwitchCam();
            PlayMng.instance.BeCaught(gameObject);
        }

        void Freeze()
        {
            isFreeze = !isFreeze;
            owner.photonView.RPC("FreezeRigid", RpcTarget.All);
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.gameObject.layer == LayerMask.NameToLayer("Weapon"))
            {
                Damaged();
            }
        }

    }
}