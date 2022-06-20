using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace DH
{
    public class TaggerController : Controller
    {
        private void Awake()
        {
            CameraMng.instance.TaggerCamSetting();
        }
        public override void ControlUpdate()
        {
            move.GroundChecker();
        }


        public override void ControllerAction()
        {
            owner.photonView.RPC("Attack", RpcTarget.All);
        }
    }
}
