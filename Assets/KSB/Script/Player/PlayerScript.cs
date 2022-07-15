using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using UnityEngine.SceneManagement;

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

        [SerializeField]
        Transform charactorBody = null;

        [SerializeField]
        GameObject dieVFX;

        Controller control = null;
        PhotonView view;

        bool isSettingComplete = false;

        bool animBool;

        #endregion
        #region public

        public UnityAction freezeAction;

        public int ownerID = -1;

        public bool isFreeze = false;

        public bool isDead = false;
        
        #endregion


        private void OnEnable()
        {
            rigid = GetComponent<Rigidbody>();
            if (photonView.IsMine)
            {
                CameraMng.instance.PlayerCamSetting(transform.GetChild(0).gameObject);
                view = GetComponent<PhotonView>();
            }
            if (SceneManager.GetActiveScene().name == "LobbyScene")
                Destroy(gameObject);
        }

        public void ControllerSetting()
        {
            control = GetComponent<Controller>();
            control.Setting(rigid);
            photonView.RPC("MyObjectsSetting", RpcTarget.AllBuffered);
        }

        [PunRPC]
        public void MyObjectsSetting()
        {
            taggerAnim = transform.GetChild(2).GetComponent<Animator>() == null ? null : transform.GetChild(2).GetComponent<Animator>();
            runnerAnim = taggerAnim != null ? null : transform.GetComponent<Animator>();
            if (runnerAnim != null)
                runnerAnim.enabled = true;
            if (photonView.IsMine)
                ownerID = PhotonNetwork.LocalPlayer.GetPlayerNumber();
            charactorBody = transform.GetChild(2).transform;
            isSettingComplete = true;
        }

        private void Update()
        {
            if (!isSettingComplete)
                return;
            if (!photonView.IsMine)
            {
                if(null == charactorBody)
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

        [PunRPC]
        public void KickTheCan(Vector3 vec, Player p)
        {
            PlayMng.instance.KickTheCan(vec, p);
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
        private void OnApplicationQuit()
        {
            if (photonView.IsMine)
            {
                PhotonNetwork.RemoveRPCs(view);
                //PhotonNetwork.Destroy(view);
            }
                //PhotonNetwork.OpCleanRpcBuffer(GetComponent<PhotonView>());
        }
    }
}
