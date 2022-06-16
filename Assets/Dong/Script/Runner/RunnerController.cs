using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    }
}