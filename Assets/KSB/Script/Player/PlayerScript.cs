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

        public int ownerID = -1;

        bool isSettingComplete = false;

        bool animBool;

        private void Awake()
        {
            playerSceneInfo = GameObject.FindGameObjectWithTag("DontDestroy").GetComponent<PlayerSceneInfo>();
            rigid = GetComponent<Rigidbody>();
            if (photonView.IsMine)
                CameraMng.instance.PlayerCamSetting(transform.GetChild(0).gameObject);
        }

        private void Start()
        {
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
        [PunRPC]
        public void KickTheCan(Vector3 vec, Player p)
        {
            Debug.Log("Kick");
            PlayMng.instance.KickTheCan(vec, p);
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
                taggerAnim?.SetBool("isMove", animBool);
                animBool = (bool)stream.ReceiveNext();
                taggerAnim?.SetBool("isJump", animBool);
                animBool = (bool)stream.ReceiveNext();
                runnerAnim?.SetBool("isDie", animBool);
                    
                ownerID = (int)stream.ReceiveNext();
                if (null != charactorBody)
                    charactorBody.rotation = (Quaternion)stream.ReceiveNext();
                else
                    quaternion = (Quaternion)stream.ReceiveNext();
            }

            // TODO : 변수 동기화

            
        }
    }
}
