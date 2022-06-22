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

        [SerializeField]
        Transform charactorBody = null;

        Controller control = null;

        public int ownerID = -1;

        bool isSettingComplete = false;

        bool animBool;

        private void Awake()
        {
            rigid = GetComponent<Rigidbody>();
            if (photonView.IsMine)
                CameraMng.instance.PlayerCamSetting(transform.GetChild(0).gameObject);
        }

        private void Start()
        {
            anim = transform.GetChild(1).GetComponent<Animator>() == null ? null : transform.GetChild(1).GetComponent<Animator>();
            if (photonView.IsMine)
                ownerID = PhotonNetwork.LocalPlayer.GetPlayerNumber();
        }

        public void ControllerSetting()
        {
            charactorBody = transform.GetChild(1).transform;
            control = GetComponent<Controller>();
            control.Setting(rigid, anim);
            isSettingComplete = true;
        }

        private void Update()
        {
            if (!photonView.IsMine)
            {
                if(isSettingComplete && null == charactorBody)
                    charactorBody = transform.GetChild(1).transform;
                return;
            }

            control?.ControlUpdate();
        }

        [PunRPC]
        public void FreezeRigid()
        {
            rigid.isKinematic = !rigid.isKinematic;
        }

        [PunRPC]
        public void Attack()
        {
            anim?.SetTrigger("isAttack");
        }
        

        public void MoveAnim(bool isMove)
        {
            anim?.SetBool("isMove", isMove);
        }

        public void JumpAnim(bool isJump)
        {
            anim?.SetBool("isJump", isJump);
        }
        Quaternion quaternion;

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(isSettingComplete);
                stream.SendNext(anim?.GetBool("isMove") == null ? false : anim.GetBool("isMove"));
                stream.SendNext(anim?.GetBool("isJump") == null ? false : anim.GetBool("isJump"));
                stream.SendNext(ownerID);
                if (isSettingComplete)
                    stream.SendNext(charactorBody.rotation);
                else
                    stream.SendNext(Quaternion.identity);
            }
            else
            {
                isSettingComplete = (bool)stream.ReceiveNext();
                animBool = (bool)stream.ReceiveNext();
                anim?.SetBool("isMove", animBool);
                animBool = (bool)stream.ReceiveNext();
                anim?.SetBool("isJump", animBool);
                ownerID = (int)stream.ReceiveNext();
                if (null != charactorBody)
                    charactorBody.rotation = (Quaternion)stream.ReceiveNext();
                else
                    quaternion = (Quaternion)stream.ReceiveNext();
            }
        }
    }
}
