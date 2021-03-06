using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;


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


        private void OnEnable()
        {
            text.text = "";
        }

        public void RoomChatSendClicked()
        {

            if (inputfield.text == "")
                return;
            photonView.RPC("RoomChatMessage",
                           RpcTarget.All,
                           PhotonNetwork.LocalPlayer.NickName,
                           inputfield.text,
                           YSM.YSMGameManager.instance.GetLocalPlayerNumbering(),
                           PhotonNetwork.IsMasterClient
                           ) ;
            inputfield.text = "";
        }

        [PunRPC]
        public void RoomChatMessage(string a, string b,PlayerColorType colorIdx, bool isHost = false)
        {
 
            
            if (isHost) //방장채팅 구분
            {
                
                text.text += "\n" + "<color=#" + ColorTransform.EnumToTextString(colorIdx) + ">" + "★ </color>" +
                              "<color=#" + ColorTransform.EnumToTextString(PlayerColorType.WHITE) + ">"+ a + "</color>" + " : "; //채팅 색상 변경
                text.text += b;

            }
            else
            {
                text.text += "\n" + "<color=#" + ColorTransform.EnumToTextString(colorIdx) + ">" + "● </color>" + a + " : "; //채팅 색상 변경
                text.text += b;
            }

        }
        
    }
}