using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectScript : MonoBehaviour
{
    //[SerializeField]
    //GameObject[] list;

    //private int count;
    //private int random;

    public void ActiveChild(int index)
    {
        //count = transform.childCount;
        //list = new GameObject[count];

        //for (int i = 0; i < count; i++)
        //{
        //    list[i] = transform.GetChild(i).gameObject;
        //}

        //random = Random.Range(0, count);

        //list[random].SetActive(true);
        transform.GetChild(index).gameObject.SetActive(true);
    }


}
