using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace YSM
{
    //룸에서 채팅 , 귓속말, 길드채팅등 추가할 수 있으니 따로 나누었다.
    enum RoomChatType  
    {
        CHAT_ROOM,
    }

    public class RoomChat : MonoBehaviourPun
    {
        [SerializeField] InputField inputfield; // 입력 텍스트
        [SerializeField] private Text text;     // 게임에 보여줄 텍스트
        private string chat;  //내가 보낼 텍스트




        public void ClickChatMessage()
        {
            if (inputfield.text == "")
                return;
            photonView.RPC("ChatMessage",
                           RpcTarget.All,
                            PhotonNetwork.LocalPlayer.NickName,
                           inputfield.text);
            inputfield.text = "";
        }

        [PunRPC]
        public void ChatMessage(string a, string b)
        {
            
            chat = string.Format("{0} : {1}", a, b);
            text.text += "\n"+ "<color=#999922>" + chat + "</color>";
            chat = "";
        }
    }
}