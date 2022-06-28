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
        [SerializeField]
        private Animator taggerAnim = null;
        [SerializeField]
        private Animator runnerAnim = null;
        private PlayerSceneInfo playerSceneInfo;

        [SerializeField]
        Transform charactorBody = null;

        [SerializeField]
        GameObject dieVFX;

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

            taggerAnim = transform.GetChild(2).GetComponent<Animator>() == null ? null : transform.GetChild(2).GetComponent<Animator>();
            runnerAnim = taggerAnim != null ? null : transform.GetComponent<Animator>();
            if (runnerAnim != null)
                runnerAnim.enabled = true;
            if (photonView.IsMine)
                ownerID = PhotonNetwork.LocalPlayer.GetPlayerNumber();
        }

        public void ControllerSetting()
        {
            charactorBody = transform.GetChild(2).transform;
            control = GetComponent<Controller>();
            control.Setting(rigid, taggerAnim);
            isSettingComplete = true;
        }

        private void Update()
        {
            if (!photonView.IsMine)
            {
                if(isSettingComplete && null == charactorBody)
                    charactorBody = transform.GetChild(2).transform;
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
            taggerAnim?.SetTrigger("isAttack");
        }

        public void DieAnim()
        {
            runnerAnim?.SetBool("isDie",true);
        }

        public void DieVFX()
        {
            charactorBody.gameObject.SetActive(false);
            dieVFX.SetActive(true);
            StartCoroutine(Die());
        }

        IEnumerator Die()
        {
            yield return new WaitForSeconds(0.5f);
            if(photonView.IsMine)
            {
                CameraMng.instance.SwitchCam();
                PlayMng.instance.BeCaught(gameObject);
            }
        }

        public void MoveAnim(bool isMove)
        {
            taggerAnim?.SetBool("isMove", isMove);
        }

        public void JumpAnim(bool isJump)
        {
            taggerAnim?.SetBool("isJump", isJump);
        }
        Quaternion quaternion;

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(isSettingComplete);
                stream.SendNext(taggerAnim == null ? false : taggerAnim.GetBool("isMove"));
                stream.SendNext(taggerAnim == null ? false : taggerAnim.GetBool("isJump"));
                stream.SendNext(runnerAnim == null ? false : runnerAnim.GetBool("isDie"));
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
                if (taggerAnim != null)
                    taggerAnim.SetBool("isMove", animBool);
                animBool = (bool)stream.ReceiveNext();
                if (taggerAnim != null)
                    taggerAnim.SetBool("isJump", animBool);
                animBool = (bool)stream.ReceiveNext();
                if (runnerAnim != null)
                    runnerAnim.SetBool("isDie", animBool);
                    
                ownerID = (int)stream.ReceiveNext();
                if (null != charactorBody)
                    charactorBody.rotation = (Quaternion)stream.ReceiveNext();
                else
                    quaternion = (Quaternion)stream.ReceiveNext();
            }
        }
    }
}
