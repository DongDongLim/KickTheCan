using Firebase;
using Firebase.Database;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class FriendPanel : MonoBehaviour
{

  
    bool isExistNickname;
    [SerializeField] private InputField requestNicknameField;
    [SerializeField] private string requestFriendUID;

    //UI

    [SerializeField] GameObject friendPanel;
    [SerializeField] GameObject requestPanel;

    [SerializeField] GameObject userNameWrong;



    public void RequestFriend()
    {
        //내가 친구 요청을걸면 건 상대의 UID에 
        DatabaseManager.instance.dbReference
            .Child("UserInfo")
            .Child(requestFriendUID/*상대방 UID넣어주고*/)
            .Child(DBFriend.Friend/*Friend*/)
            .Child(DBFriend.FriendRequests /*DB Friend Request List에 넣어주기*/)
            .UpdateChildrenAsync(FuncTool.ConvertToIDictionary(DatabaseManager.instance.dbData.DisplayNickname, AuthManager.instance.GetCurrentUID())); /*내정보*/
        
    }

    #region Client Friend request
    public void FriendRequestClicked()
    {

        FindUserNickname(FriendRequestCheck);

    }

    public void FindUserNickname(UnityAction OnCheck)
    {
        DatabaseManager.instance.reference = FirebaseDatabase.DefaultInstance.GetReference("UserInfo");

        DatabaseManager.instance.reference.GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                isExistNickname = false;
                requestFriendUID = null;
                foreach (DataSnapshot data in snapshot.Children)
                {

                    IDictionary userInfo = (IDictionary)data.Value;
                    if ((string)userInfo["DisplayNickname"] == requestNicknameField.text)
                    {
                        requestFriendUID = data.Key;
                        Debug.Log(requestFriendUID);
                        isExistNickname = true;
                        break;
                    }
                }
                OnCheck.Invoke();
            }
        });
    }

    public void FriendRequestCheck()
    {
        if (isExistNickname)
        {
            Debug.Log("요청 성공");
            RequestFriend();
        }
        else
        {
            userNameWrong.SetActive(true);
            Debug.Log("없는사람");
        }

    }

    #endregion


    #region Friend Receive , Cansel
    public void ReceiveFriendRequest(FriendRequestEntry entry)
    {
        Debug.Log(entry.GetRequestFriendName());
        //내가 친구요청을 받으면 받은 아이디를 내 FriendList에 Nickname, UID넣어주고 
        DatabaseManager.instance.dbReference
            .Child("UserInfo")
            .Child(AuthManager.instance.GetCurrentUID()/*내 UID에*/)
            .Child(DBFriend.Friend/*Friend*/)
            .Child(DBFriend.FriendLists /*DB Friend List에 넣어주기*/)
            .UpdateChildrenAsync(FuncTool.ConvertToIDictionary(entry.GetRequestFriendName() ,entry.GetUID())/*넣을 데이터*/);

        //내가 받은 친구요청을 상대방에게도 추가해주고
        DatabaseManager.instance.dbReference
             .Child("UserInfo")
             .Child(entry.GetUID()/*상대방 UID넣어주고*/)
             .Child(DBFriend.Friend/*Friend*/)
             .Child(DBFriend.FriendLists /*DB Friend List에 넣어주기*/)
             .UpdateChildrenAsync(FuncTool.ConvertToIDictionary(DatabaseManager.instance.dbData.DisplayNickname, AuthManager.instance.GetCurrentUID())/*내정보*/);

        //내 request요청에 있는거 지워주고
        DatabaseManager.instance.dbReference
             .Child("UserInfo")
             .Child(AuthManager.instance.GetCurrentUID()/*내 UID에 있는거 지우고*/)
             .Child(DBFriend.Friend/*Friend*/)
             .Child(DBFriend.FriendRequests /*DB Friend Request List에 있는 데이터 지워주기*/)
             .Child(entry.GetRequestFriendName()/*지울 데이터*/)
             .RemoveValueAsync();

        Destroy(entry.gameObject);
    }
    
    public void CancelFriendRequest(FriendRequestEntry entry)
    {
        Debug.Log(entry.GetRequestFriendName());
        DatabaseManager.instance.dbReference
             .Child("UserInfo")
             .Child(AuthManager.instance.GetCurrentUID()/*내 UID에 있는거 지우고*/)
             .Child(DBFriend.Friend/*Friend*/)
             .Child(DBFriend.FriendRequests /*DB Friend Request List에 있는 데이터 지워주기*/)
             .Child(entry.GetRequestFriendName()/*지울 데이터*/)
             .RemoveValueAsync();
        Destroy(entry.gameObject);
    }

    public void OpenFriendInfo(FriendListEntry entry)
    {
        Debug.Log("OpenClicked");
    }


    #endregion

    #region UI event

    public void OnFriendListOpenBtnClicked()
    {
        friendPanel.SetActive(true);
    }

    public void OnFriendListCloseBtnClicked()
    {
        friendPanel.SetActive(false);
    }

    public void OnRequestListCloseBtnClicked()
    {

        requestPanel.SetActive(false);
    }
    public void OnRequestListOpenBtnClicked()
    {
        requestPanel.SetActive(true);
    }

    #endregion


}
