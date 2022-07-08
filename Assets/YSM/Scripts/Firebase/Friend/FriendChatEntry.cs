using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FriendChatEntry : MonoBehaviour
{
    [SerializeField] private Text time;
    [SerializeField] private Text username;
    [SerializeField] private Text message;

    public void SetData(string time , string username, string message ,bool isMine)
    {
        this.time.text = time;
        this.username.text = username;
        this.message.text = message;

        if(isMine)
        {
            this.time.alignment = TextAnchor.MiddleRight;
            this.username.alignment = TextAnchor.MiddleRight;
            this.message.alignment = TextAnchor.MiddleRight;
            this.time.color = Color.black;
            this.username.color = Color.black;
            this.message.color = Color.black;
        }
        else
        {
            this.time.alignment = TextAnchor.MiddleLeft;
            this.username.alignment = TextAnchor.MiddleLeft;
            this.message.alignment = TextAnchor.MiddleLeft;
            this.time.color = Color.black;
            this.username.color = Color.black;
            this.message.color = Color.black;
        }
    }

}
