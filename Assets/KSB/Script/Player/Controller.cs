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

        public void Setting(Rigidbody r, Animator animator)
        {
            owner = GetComponent<PlayerScript>();
            move = GetComponent<PlayerMove>();
            rigid = r;
            move.Setting(rigid);
            UIMng.instance.SetMoveUI(move);
            UIMng.instance.actionAction += ControllerAction;
        }

        private void OnDestroy()
        {
            if (UIMng.instance != null)
                UIMng.instance.actionAction -= ControllerAction;
        }

        public abstract void ControlUpdate();

        public abstract void ControllerAction();

    }
}
