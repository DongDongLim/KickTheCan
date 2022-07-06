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


    public void OnCloseBtnClicked()
    {
        gameObject.SetActive(false);
    }

    public void WrongNickname(string name)
    {
        text.text = "\"" + name + "\"\n" + "사용자이름이 잘못되었습니다";
    }

    public void AlreadyFriend(string name)
    {
        text.text = "\"" + name + "\"\n" + "이미 친구입니다";
    }
    
    public void RequestSuccese(string name)
    {
        text.text = "\"" + name + "\"\n" + "요청 성공";
    }

}
