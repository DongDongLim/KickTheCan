using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

namespace DH
{
    public class CanSetScript : MonoBehaviourPun
    {
        [SerializeField]
        GameObject can;

        private void OnEnable()
        {
            if (SceneManager.GetActiveScene().name == "LobbyScene")
                Destroy(gameObject);
            else
                ChildObjCreate();
        }

        public void ChildObjCreate()
        {
            Instantiate(can, transform, false);
            PlayMng.instance.can = gameObject;
        }
    }
}