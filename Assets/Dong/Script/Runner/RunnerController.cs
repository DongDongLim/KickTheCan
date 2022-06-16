using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace DH
{
    public class RunnerController : Controller
    {
        bool isFreeze = false;
        public override void ControlUpdate()
        {
            if (!isFreeze)
            {
                move.Move();
                move.Jump();
            }
        }

        public void OnFreezeButton()
        {
            Freeze();
        }

        void Freeze()
        {
            isFreeze = !isFreeze;
            owner.photonView.RPC("FreezeRigid", RpcTarget.All);
        }

    }
}