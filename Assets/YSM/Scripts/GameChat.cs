using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace YSM
{

    public enum GameChatColor
    {

    }


    public enum GameCharacterType //캐릭터 상태
    {
        RUNNER,   //순서만 같으면 
        TAGGER,   //자기랑만 상호작용 
        DEAD,     // 러너랑 상호작용
        OBSERVER, //모든 채팅 다 보게끔
        CNT,
    }

    public enum GameChatType //게임 내에서 채팅
    {
        ALL, //전체 채팅
        TEAM, //팀 채팅
        DEAD, //죽은사람 채팅
        OBSERVER, //옵저버 채팅
        NOTICE, //공지 채팅
    }

    public class GameChat : MonoBehaviourPun
    {

        [SerializeField] InputField inputfield;     // 입력 텍스트
        [SerializeField] private Text allText;      // 전쳇 게임에 보여줄 텍스트
        [SerializeField] private Text teamText;     // 팀 쳇 게임에 보여줄 텍스트


        [SerializeField] private GameCharacterType curCharacterType;
        [SerializeField] private GameChatType curChatType;
        [SerializeField] private Button typeButton;
        [SerializeField] private Text typeButtonText;

        [SerializeField] private Text curCharacterTypeText;

        string[] charImageType = {"♥" , "★", "♣","♠" };




        private void OnEnable()
        {
            //text.text = "";
            curChatType = GameChatType.ALL;
        }


        public void ClickTypeButton()
        {

            if (GameChatType.DEAD == curChatType)
            {
                typeButtonText.text = curChatType.ToString();
                return;
            }
            Debug.Log(curChatType);
            curChatType = GameChatType.ALL == curChatType ? GameChatType.TEAM : GameChatType.ALL;
            typeButtonText.text = curChatType.ToString();
        }

        public void TestChangeCharacterClicked()
        {
            
            curCharacterType++;
            curCharacterType = (GameCharacterType)((int)curCharacterType % (int)GameCharacterType.CNT);
            curCharacterTypeText.text = curCharacterType.ToString();
        }

        public void GameChatSendClicked()
        {
            if (inputfield.text == "")
                return;
            photonView.RPC("GameChatMessage",
                           RpcTarget.All,
                           PhotonNetwork.LocalPlayer.NickName,
                           inputfield.text,
                           YSM.YSMGameManager.instance.GetLocalPlayerNumbering(),
                           curCharacterType,
                           curChatType,
                           PhotonNetwork.IsMasterClient
                           );
            inputfield.text = "";
        }

        [PunRPC]
        public void GameChatMessage(string userNickName, string userMessage, PlayerColorType colorIdx, GameCharacterType receiveCharacterType, GameChatType receiveChatType, bool isHost = false)
        {
            Debug.Log("캐릭터 타입 받음 : "+receiveCharacterType.ToString()+ "  /// " + "내꺼"+curCharacterType.ToString());
            Debug.Log("캐릭터 챗 타입 받음 : "+ receiveChatType.ToString()+ "  /// " + "내꺼"+ curChatType.ToString());

            if (curCharacterType == GameCharacterType.OBSERVER) //옵저버면 모든 채팅 받기
            {}
            else if(receiveChatType == GameChatType.NOTICE) // 공지 채팅 오면 모두 받기
            {}
            else if(receiveChatType == GameChatType.OBSERVER) //옵저버 채팅이 왔을 때 
            {
                if (curCharacterType != GameCharacterType.OBSERVER) //옵저버가 아니면 받지 않음
                    return; //return 이면 못받음
            }
            else if (receiveChatType == GameChatType.TEAM)
            {
                if (receiveCharacterType == GameCharacterType.RUNNER) //러너가 팀챗을 보냈을때 못받는것
                {
                    Debug.Log("1");
                    if (RunnerSendTeamChat())
                        return; 
                }
                else if (receiveCharacterType == GameCharacterType.TAGGER) // 테거가 팀챗을 보냈을때 못받는것
                { 
                    Debug.Log("2");
                    if (TaggerSendTeamChat())
                        return;
                }
                else if (receiveCharacterType == GameCharacterType.DEAD) // 죽은 사람이 채팅 보냈을때 못받는것
                {
                    Debug.Log("3");
                    if (DeadSendChat())
                        return;
                }
            }



            if (isHost) //방장채팅 구분
            {
                allText.text += "\n" + "<Size=15><color=#" + ColorTransform.EnumToTextString(colorIdx) + ">" + charImageType[(int)receiveCharacterType] + "</color></Size>" +
                    "<Size=15><color=#" + GameCharacterTypeColorToString(receiveCharacterType)+ ">" + userNickName + "  Host </color></Size>";
                allText.text += userMessage;

            }
            else
            {
                allText.text += "\n" + "<Size=15><color=#" + ColorTransform.EnumToTextString(colorIdx) + ">" + charImageType[(int)receiveCharacterType] + "</color></Size>" +
                    "<Size=15><color=#" + GameCharacterTypeColorToString(receiveCharacterType)+ ">" + userNickName + "</color></Size>";
                allText.text += userMessage;
            }


        }




        public bool RunnerSendTeamChat()
        {
            Debug.Log("러너 챗 받음");

            if (curCharacterType == GameCharacterType.TAGGER)
                return true;
            return false;
        }

        
        public bool TaggerSendTeamChat()
        {
            Debug.Log("술래 챗 받음");

            if (curCharacterType == GameCharacterType.RUNNER || curCharacterType == GameCharacterType.DEAD)
                return true;

            return false;
        }

        public bool DeadSendChat()
        {
            Debug.Log("죽음 챗 받음");

            if (curCharacterType != GameCharacterType.DEAD)
                return true;

            return false;
        }



         
        //static public Color GameCharacterTypeColor(GameCharacterType index)
        //{
        //    switch (index)
        //    {
        //        case GameCharacterType.RUNNER:           return new Color32(255, 255, 255, 255);
        //        case GameCharacterType.TAGGER:           return new Color32(255, 0, 255, 255);
        //        case GameCharacterType.DEAD:             return new Color32(127, 127, 127, 255);
        //        case GameCharacterType.OBSERVER:         return new Color32(0, 0, 0, 255);
        //        default: return Color.cyan; //오류:
        //    }
        //}
        static public string GameCharacterTypeColorToString(GameCharacterType index)
        {
             switch (index)
             {
                 case GameCharacterType.RUNNER:             return Convert.ToString(255,16) + Convert.ToString(255,16) + Convert.ToString(255,16);
                 case GameCharacterType.TAGGER:             return Convert.ToString(255,16) + "0"+Convert.ToString(0,16) + Convert.ToString(255,16);
                 case GameCharacterType.DEAD:               return Convert.ToString(127,16) + Convert.ToString(127,16) + Convert.ToString(127,16);
                 case GameCharacterType.OBSERVER:           return Convert.ToString(0,  16) + Convert.ToString(0,  16) + Convert.ToString(0,  16);
                 default: return "Error";
             }

        }




    }
}