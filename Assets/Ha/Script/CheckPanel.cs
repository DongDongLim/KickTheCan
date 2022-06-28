using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckPanel : MonoBehaviour
{


    [SerializeField] private Text text;


    public void CanUse(string name , bool canUse)
    {
        if(canUse )
        {
            text.text = name + "사용이 가능합니다";
        }
        else
        {
            text.text = name + "사용이 불가합니다";
        }
    }

    public void OnOkayButtonClicked()
    {
        text.text = "사용하시겠습니까?";
    }
}
