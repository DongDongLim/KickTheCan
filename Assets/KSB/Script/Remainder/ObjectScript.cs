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
}
