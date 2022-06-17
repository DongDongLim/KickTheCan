using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace KSB
{
    public abstract class Controller : MonoBehaviourPun
    {
        [SerializeField]
        protected PlayerScript owner;

        [SerializeField]
        protected Rigidbody rigid;

        [SerializeField]
        protected PlayerMove move;

        [SerializeField]
        protected Animator animator;

        [SerializeField]
        protected GameObject attackColl;

        public void Setting(Rigidbody r)
        {
            owner = GetComponent<PlayerScript>();
            move = GetComponent<PlayerMove>();
            rigid = r;
            animator = transform.GetChild(1).GetComponent<Animator>() == null ? null : transform.GetChild(1).GetComponent<Animator>();
            attackColl = transform.GetChild(1).childCount > 0 ? transform.GetChild(1).GetChild(0).gameObject : null;
            move.Setting(rigid, animator);
            UIMng.instance.SetMoveUI(move);
            UIMng.instance.actionAction += ControllerAction;
        }

        public abstract void ControlUpdate();

        public abstract void ControllerAction();

    }
}
