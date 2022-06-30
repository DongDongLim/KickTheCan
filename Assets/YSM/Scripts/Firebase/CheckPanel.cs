using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckPanel : MonoBehaviour
{


    [SerializeField] private Text text;


    public void CanUse(string name , bool canUse)
    {
        if(canUse)
        {
            text.text = "\"" + name + "\"\n"  + "사용 가능";
        }
        else
        {
            text.text = "\"" + name + "\"\n" + "사용 불가";
        }
    }

}
