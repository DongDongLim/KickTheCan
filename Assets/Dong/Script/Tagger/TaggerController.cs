using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace DH
{
    public class TaggerController : Controller
    {
        public override void ControlUpdate()
        {
            move.GroundChecker();
        }


        public override void ControllerAction()
        {
            //Attack();
        }
    }
}
