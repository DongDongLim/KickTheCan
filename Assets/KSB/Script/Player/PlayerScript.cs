using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Photon.Pun;
using Photon.Pun.UtilityScripts;

namespace DH
{
    public class PlayerScript : MonoBehaviourPun, IPunObservable
    {
        #region private
        private Rigidbody rigid;
        private Animator anim;
        private PlayerSceneInfo playerSceneInfo;

        [SerializeField]
        Transform charactorBody = null;

        Controller control = null;


        bool isSettingComplete = false;

        bool animBool;

        #endregion
        #region public

        public UnityAction freezeAction;

        public int ownerID = -1;

        public bool isFreeze = false;
        #endregion

        private void Awake()
        {
            playerSceneInfo = GameObject.FindGameObjectWithTag("DontDestroy").GetComponent<PlayerSceneInfo>();
            rigid = GetComponent<Rigidbody>();
            if (photonView.IsMine)
                CameraMng.instance.PlayerCamSetting(transform.GetChild(0).gameObject);
        }

        private void Start()
        {
            // TODO : 중간 입장시 발동되면 Bug 발생 
            //if (playerSceneInfo.isObserver || playerSceneInfo.isRenegade)
            //{
            //    return;
            //}

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
            isFreeze = !isFreeze;
            freezeAction?.Invoke();
            rigid.isKinematic = !rigid.isKinematic;
        }

        [PunRPC]
        public void Attack()
        {
            anim?.SetTrigger("isAttack");
        }

        [PunRPC]
        public void KickTheCan(Vector3 vec)
        {
            Debug.Log("Kick");
            PlayMng.instance.KickTheCan(vec);
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
