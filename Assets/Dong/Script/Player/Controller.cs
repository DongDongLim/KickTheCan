using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace DH
{
    public abstract class Controller : MonoBehaviourPun
    {
        protected PlayerScript Aowner;
        protected Rigidbody rigid;
        protected PlayerMove move;
        public void Setting(Rigidbody r)
        {
            Aowner = GetComponent<PlayerScript>();
            move = GetComponent<PlayerMove>();
            rigid = r;
            move.Setting(rigid);
        }

        public abstract void ControlUpdate();

    }
}
