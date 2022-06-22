using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ObjectScript : MonoBehaviourPun
{
        [SerializeField]
        GameObject[] list;

        private int count;
        private int random;

        private void Awake() {
            count = transform.childCount;
            list = new GameObject[count];
            
            for (int i = 0; i < count; i++)
            {
                list[i] = transform.GetChild(i).gameObject;
            }

            random = Random.Range(0,count);

            list[random].SetActive(true);
        }


        public void SetObjIndex(int index)
        {
            photonView.RPC("ChildObjCreate", RpcTarget.All, index);
        }

        [PunRPC]
        public void ChildObjCreate(int index)
        {
            //objIndex = index;
            //Instantiate(DH.MapSettingMng.instance.mapObj[objIndex], transform, false);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if(collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                GetComponent<Rigidbody>().isKinematic = true;
            }
        }
}
