using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;

namespace DH
{
    public class PlayerScript : MonoBehaviourPun, IPunObservable
    {
        private Rigidbody rigid;
        private Animator anim;

        Controller control = null;

        private void Awake()
        {
            rigid = GetComponent<Rigidbody>();
            if (photonView.IsMine)
                CameraMng.instance.PlayerCamSetting(transform);
        }

        public void ControllerSetting()
        {
            control = GetComponent<Controller>();
            control.Setting(rigid);
        }

        private void Update()
        {
            if (!photonView.IsMine)
                return;

            control?.ControlUpdate();
        }

        [PunRPC]
        public void FreezeRigid()
        {
            rigid.isKinematic = !rigid.isKinematic;
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                //stream.SendNext(health);
            }
            else
            {
                //health = (float)stream.ReceiveNext();
            }
        }
    }
}
