using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace YSM
{

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
        DEAD //죽은사람 채팅
    }

    public class GameChat : MonoBehaviourPun
    {

        [SerializeField] InputField inputfield; // 입력 텍스트
        [SerializeField] private Text text;     // 게임에 보여줄 텍스트


        [SerializeField] private GameCharacterType curCharacterType;
        [SerializeField] private GameChatType curChatType;
        [SerializeField] private Button typeButton;
        [SerializeField] private Text typeButtonText;

        [SerializeField] private Text curCharacterTypeText;


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
            { }
            else if (receiveChatType == GameChatType.ALL) //전채채팅이면 모두다 받기
            { }
            else if (receiveCharacterType ==GameCharacterType.RUNNER) //러너가 팀챗을 보냈을때 못받는것
                if(RunnerSendTeamChat())
                    return; //return 이면 못받음
            else if (receiveCharacterType == GameCharacterType.TAGGER) // 테거가 팀챗을 보냈을때 못받는것
                if(TaggerSendTeamChat())
                    return;
            else if (receiveCharacterType == GameCharacterType.DEAD) // 테거가 팀챗을 보냈을때 못받는것
                if(DeadSendChat())
                    return;


            if (isHost) //방장채팅 구분
            {

                text.text += "\n" + "<Size=15><color=#" + ColorTransform.EnumToTextString(colorIdx) + ">" + "★ </color></Size>" +
                              "<color=#" + ColorTransform.EnumToTextString(PlayerColorType.WHITE) + ">" + userNickName + "</color>" + " : "; //채팅 색상 변경
                text.text += userMessage;

            }
            else
            {
                text.text += "\n" + "<Size=15><color=#" + ColorTransform.EnumToTextString(colorIdx) + ">" + "● </color></Size>" + userNickName + " : "; //채팅 색상 변경
                text.text += userMessage;
            }


        }

        public bool RunnerSendTeamChat()
        {
            if (curCharacterType == GameCharacterType.TAGGER)
                return true;
            
            return false;
        }

        
        public bool TaggerSendTeamChat()
        {
            if (curCharacterType == GameCharacterType.RUNNER || curCharacterType == GameCharacterType.DEAD)
                return true;
            
            return false;
        }

        public bool DeadSendChat()
        {
            if (curCharacterType != GameCharacterType.DEAD)
                return true;

            return false;
        }



    }
}