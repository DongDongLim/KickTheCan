using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SignUpComplete : MonoBehaviour
{

    [SerializeField] private Text text;


    public void SetNameText(string name )
    {
        text.text = name + "회원가입이 완료되었습니다.";
    }
    public void OKButtonClicked()
    {
        transform.parent.gameObject.SetActive(false);
    }
}
