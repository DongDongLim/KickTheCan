using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatEntry : MonoBehaviour
{
    [SerializeField] Image iconImage;
    [SerializeField] Text nameText;
    [SerializeField] Text message;




    public void SetData(Sprite sprtie,Color idxColor, string chatName, string chatMessage)
    {
        iconImage.sprite = sprtie;
        iconImage.color = idxColor;
        nameText.text = chatName;
        message.text = chatMessage;
    }


}
