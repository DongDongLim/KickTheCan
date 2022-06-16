using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace DH
{
    public abstract class Controller : MonoBehaviourPun
    {
        [SerializeField]
        protected PlayerScript owner;

        [SerializeField]
        protected Rigidbody rigid;

        [SerializeField]
        protected PlayerMove move;


        public void Setting(Rigidbody r)
        {
            owner = GetComponent<PlayerScript>();
            move = GetComponent<PlayerMove>();
            rigid = r;
            move.Setting(rigid);
        }

        public abstract void ControlUpdate();

    }
}
