using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System;
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
        NOTICE,   //공지 채팅
        CNT,
    }

    public enum GameChatType //게임 내에서 채팅
    {
        ALL, //전체 채팅
        TEAM, //팀 채팅
        DEAD, //죽은사람 채팅
        OBSERVER, //옵저버 채팅
        NOTICE, //공지 채팅
        CNT,
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
        [SerializeField] private Text curChatTypeText;


        [SerializeField] private GameObject chatEntryPrefab;
        [SerializeField] private GameObject allChatContent;
        [SerializeField] private GameObject teamChatContent;
        [SerializeField] private GameObject systemChatContent;

        [SerializeField] private Sprite[] charImageType;
        ChatEntry chatEntry;


        Color tmpColor = new Color();
        string tmpChatName;
        string tmpChatMessage;



        private void OnEnable()
        {

            curChatType = GameChatType.ALL;
            chatEntry = chatEntryPrefab.GetComponent<ChatEntry>();
        }

        private void OnDisable()
        {
            curChatType = GameChatType.ALL;
            chatEntry = chatEntryPrefab.GetComponent<ChatEntry>();
        }


        public void ClickChatTypeButton()
        {

            //curChatType++;
            //curChatType = (GameChatType)((int)curChatType % (int)GameChatType.CNT);
            //curChatTypeText.text = curChatType.ToString();

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
            
            if (receiveChatType == GameChatType.NOTICE) // 공지 채팅 오면 모두 받기
            { }
            else if (curCharacterType == GameCharacterType.OBSERVER) //내가 옵저버면 모든 채팅 받기
            { }
            else if (receiveCharacterType == GameCharacterType.OBSERVER) //옵저버 채팅이 왔을 때 
            {
                if (curCharacterType != GameCharacterType.OBSERVER) //옵저버가 아니면 받지 않음
                {
                    return; //return 이면 못받음
                }
            }
            else if (receiveChatType == GameChatType.TEAM)
            {
                if (receiveCharacterType == GameCharacterType.RUNNER) //러너가 팀챗을 보냈을때 못받는것
                {
                    if (RunnerSendTeamChat())
                        return;
                }
                else if (receiveCharacterType == GameCharacterType.TAGGER) // 테거가 팀챗을 보냈을때 못받는것
                {
                    if (TaggerSendTeamChat())
                        return;
                }
                else if (receiveCharacterType == GameCharacterType.DEAD) // 죽은 사람이 채팅 보냈을때 못받는것
                {
                    if (DeadSendChat())
                        return;
                }
            }



            GameObject entry = Instantiate(chatEntryPrefab);

            SetColor(userNickName, userMessage,colorIdx,receiveCharacterType, receiveChatType); //컬러 세팅하고
            
            tmpChatName += isHost && receiveCharacterType != GameCharacterType.NOTICE ? " (Host)" : ""; //호스트인지 아닌지


            entry.GetComponent<ChatEntry>().SetData(
                charImageType[(int)receiveCharacterType],
                tmpColor,
                tmpChatName,
                tmpChatMessage
                );

            if (receiveCharacterType == GameCharacterType.NOTICE)
            {
                Debug.Log("시스템쳇ㅔㅔㅔㅔㅔㅔㅔ잇 추가");
                GameObject tmpEntry = Instantiate(entry);
                tmpEntry.transform.localScale = Vector3.one;
                tmpEntry.transform.SetParent(systemChatContent.transform);
            }

            else if (receiveChatType == GameChatType.TEAM)
            {
                Debug.Log("팀체ㅔㅔㅔㅔㅔㅔㅔ잇 추가");
                GameObject tmpEntry = Instantiate(entry);
                tmpEntry.transform.localScale = Vector3.one;
                tmpEntry.transform.SetParent(teamChatContent.transform);
            }



            entry.transform.localScale = Vector3.one;
            entry.transform.SetParent(allChatContent.transform);



        }


        public void SystemKillLog(Player tagger , Player runner)
        {
            photonView.RPC("AddKillLog",
               RpcTarget.All,
               tagger,
               runner
               );
        }

        [PunRPC]
        public void AddKillLog(Player tagger, Player runner)
        {
            Debug.Log(true);
            GameObject systemChatEntry = Instantiate(chatEntryPrefab);
            GameObject allChatEntry;
            //tagger.NickName;

            systemChatEntry.GetComponent<ChatEntry>().SetData(
                charImageType[(int)GameChatType.NOTICE],
                ColorTransform.EnumToColor(PlayerColorType.RED),
                "<Size=15><color=#" + GameChatTypeColorToString(GameChatType.NOTICE) + "> <System></color></Size>",
                "<Size=15><color=#" + YSM.ColorTransform.EnumToColor((YSM.PlayerColorType)tagger.GetPlayerNumber()) + ">" +tagger.NickName +"</color></Size> ->" +
                "<Size=15><color=#" + YSM.ColorTransform.EnumToColor((YSM.PlayerColorType)runner.GetPlayerNumber()) + ">" + runner.NickName +"</color></Size> Tag!!!"
                );

            systemChatEntry.transform.localScale = Vector3.one;
            allChatEntry = Instantiate(systemChatEntry);

            systemChatEntry.transform.SetParent(systemChatContent.transform);
            allChatEntry.transform.SetParent(allChatContent.transform);

        }



        public void SystemCanKickLog(Player runner)
        {
            photonView.RPC("AddCanKickLog",
               RpcTarget.All,
               runner
               );
        }

        [PunRPC]
        public void AddCanKickLog(Player runner)
        {

            GameObject systemChatEntry = Instantiate(chatEntryPrefab);
            GameObject allChatEntry;

            systemChatEntry.GetComponent<ChatEntry>().SetData(
                charImageType[(int)GameChatType.NOTICE],
                ColorTransform.EnumToColor(PlayerColorType.RED),
                "<Size=15><color=#" + GameChatTypeColorToString(GameChatType.NOTICE) + "> <System></color></Size>",
                "<Size=15><color=#" + YSM.ColorTransform.EnumToColor((YSM.PlayerColorType)runner.GetPlayerNumber()) + ">" + runner.NickName + "</color></Size> Kick Can!!!!!!!!"
                );

            systemChatEntry.transform.localScale = Vector3.one;
            allChatEntry = Instantiate(systemChatEntry);
            systemChatEntry.transform.SetParent(systemChatContent.transform);
            allChatEntry.transform.SetParent(allChatContent.transform);

        }




        private void SetColor(string userNickName, string userMessage, PlayerColorType colorIdx, GameCharacterType receiveCharacterType, GameChatType receiveChatType)
        {
            switch (receiveCharacterType)
            {
                case GameCharacterType.RUNNER:
                case GameCharacterType.TAGGER:
                    tmpColor = ColorTransform.EnumToColor(colorIdx);
                    tmpChatName = "<Size=15><color=#" + GameCharacterTypeColorToString(receiveCharacterType) + ">" + userNickName + "</color></Size>";
                    tmpChatMessage = "<Size=15><color=#" + GameChatTypeColorToString(receiveChatType) + ">" + userMessage + "</color></Size>";
                    break;
                case GameCharacterType.DEAD:
                case GameCharacterType.OBSERVER:
                    tmpColor = ColorTransform.EnumToColor(colorIdx);
                    tmpChatName = "<Size=15><color=#" + GameCharacterTypeColorToString(receiveCharacterType) + ">" + userNickName + "</color></Size>";
                    tmpChatMessage = "<Size=15><color=#" + GameCharacterTypeColorToString(receiveCharacterType) + ">" + userMessage + "</color></Size>";
                    break;
                case GameCharacterType.NOTICE:
                    tmpColor = ColorTransform.EnumToColor(PlayerColorType.RED);
                    tmpChatName = "<Size=15><color=#" + GameCharacterTypeColorToString(receiveCharacterType) + "> <System></color></Size>";
                    tmpChatMessage = "<Size=15><color=#" + GameCharacterTypeColorToString(receiveCharacterType) + ">" + userMessage + "</color></Size>";
                    break;
                default:
                    tmpChatName = "Error";
                    tmpChatMessage = "Error";
                    break;
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



        static public Color GameCharacterTypeColor(GameCharacterType index)
        {
            switch (index)
            {
                case GameCharacterType.RUNNER: return new Color32(255, 255, 255, 255);
                case GameCharacterType.TAGGER: return new Color32(255, 0, 255, 255);
                case GameCharacterType.DEAD: return new Color32(127, 127, 127, 255);
                case GameCharacterType.OBSERVER: return new Color32(0, 0, 0, 255);
                default: return Color.cyan; //오류:
            }
        }
        static public string GameCharacterTypeColorToString(GameCharacterType index)
        {
             switch (index)
             {
                 case GameCharacterType.RUNNER:             return Convert.ToString(255,16) + Convert.ToString(255,16) + Convert.ToString(255,16); //흰색
                 case GameCharacterType.TAGGER:             return Convert.ToString(0,16) + Convert.ToString(0,16) + Convert.ToString(0,16); //검정색
                 case GameCharacterType.DEAD:               return Convert.ToString(127,16) + Convert.ToString(127,16) + Convert.ToString(127,16); //회색
                 case GameCharacterType.OBSERVER:           return Convert.ToString(34, 16) + Convert.ToString(177, 16) + Convert.ToString(76, 16);   //초록색
                 case GameCharacterType.NOTICE:             return Convert.ToString(237,16) + Convert.ToString(28, 16) + Convert.ToString(36, 16);   //빨강색
                default: return "Error";
             }

        }

        static public string GameChatTypeColorToString(GameChatType index)
        {
             switch (index)
             {
                 case GameChatType.ALL:               return Convert.ToString(255,16) + Convert.ToString(255,16) + Convert.ToString(255,16); //흰색
                 case GameChatType.TEAM:              return Convert.ToString(255,16) + Convert.ToString(242,16) + "0" + Convert.ToString(0, 16); // 노랑색
                 case GameChatType.DEAD:              return Convert.ToString(127,16) + Convert.ToString(127,16) + Convert.ToString(127,16);   //회색
                 case GameChatType.OBSERVER:          return Convert.ToString(34, 16) + Convert.ToString(177,16) + Convert.ToString(76, 16);   //초록색
                 case GameChatType.NOTICE:            return Convert.ToString(237,16) + Convert.ToString(28, 16) + Convert.ToString(36, 16);   //빨강색
                 default: return "Error";
             }

        }


    }
}