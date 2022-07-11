using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace DH
{
    public class CanSetScript : MonoBehaviourPun
    {
        [SerializeField]
        GameObject can;

        private void OnEnable()
        {
            ChildObjCreate();
        }

        public void ChildObjCreate()
        {
            Instantiate(can, transform, false);
            PlayMng.instance.can = gameObject;
        }
    }
}