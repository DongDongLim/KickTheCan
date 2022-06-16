using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace DH
{
    public class RunnerController : Controller, IPunObservable
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
            isFreeze = !isFreeze;
            rigid.isKinematic = !rigid.isKinematic;
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(isFreeze);
                stream.SendNext(rigid);
            }
            else
            {
                isFreeze = (bool)stream.ReceiveNext();
                rigid = (Rigidbody)stream.ReceiveNext();
            }
        }
    }
}