using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace DH
{
    public class RunnerController : Controller
    {
        bool isFreeze = false;

        public override void ControllerAction()
        {
            Freeze();
        }

        public override void ControlUpdate()
        {
            move.GroundChecker();
        }

        void Freeze()
        {
            isFreeze = !isFreeze;
            owner.photonView.RPC("FreezeRigid", RpcTarget.All);
        }

    }
}