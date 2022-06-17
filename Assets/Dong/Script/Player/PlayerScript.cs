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

        [SerializeField]
        protected Animator animator;

        [SerializeField]
        protected GameObject attackColl;

        private void Awake()
        {
            rigid = GetComponent<Rigidbody>();
            if (photonView.IsMine)
                CameraMng.instance.PlayerCamSetting(transform.GetChild(0).gameObject);
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


        [PunRPC]
        public void Attack()
        {
            attackColl?.SetActive(true);
            animator?.SetTrigger("isAttack");
            StartCoroutine("AttackEnd");
        }

        [PunRPC]
        IEnumerator AttackEnd()
        {
            yield return new WaitForSeconds(0.1f);
            attackColl?.SetActive(false);
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
