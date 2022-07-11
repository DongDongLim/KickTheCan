using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FriendChatEntry : MonoBehaviour
{
    [SerializeField] private Text username;
    [SerializeField] private Text message;
    string time;

    public void SetData(string time , string username, string message ,bool isMine)
    {
        this.message.text = message;

        this.time = $"<size=5> {time} </size>";
        if (isMine)
        {
            this.username.alignment = TextAnchor.MiddleRight;
            this.message.alignment = TextAnchor.MiddleRight;
            this.username.color = Color.black;
            this.message.color = Color.black;
            this.username.text = $"<size=9> {time} </size> {username}";
        }
        else
        {
            this.username.alignment = TextAnchor.MiddleLeft;
            this.message.alignment = TextAnchor.MiddleLeft;
            this.username.color = Color.black;
            this.message.color = Color.black;
            this.username.text = $" {username} <size=9> {time} </size>";

        }
    }

}
