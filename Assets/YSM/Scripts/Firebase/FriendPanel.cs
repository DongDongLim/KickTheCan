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

    public DBFriend dbFriend;


    bool isExistNickname;
    [SerializeField] private InputField requestNicknameField;
    [SerializeField] private string requestFriendUID;

    void Start()
    {
    }



    public void RequestFriend()
    {
        //내가 친구 요청을걸면 건 상대의 UID에 
        DatabaseManager.instance.dbReference
            .Child("UserInfo")
            .Child(requestFriendUID/*상대방 UID넣어주고*/)
            .Child(DBFriend.Friend/*Friend*/)
            .Child(DBFriend.FriendRequests /*DB Friend Request List에 넣어주기*/)
            .UpdateChildrenAsync(StringToIDictionary(DatabaseManager.instance.dbData.DisplayNickname, AuthManager.instance.GetAuthUID())); /*내정보*/

    }



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
            Debug.Log("친구추가");
            RequestFriend();
        }
        else
        {
            Debug.Log("없는사람");
        }

    }


    public string DictionaryToJson(string nickname, string UID)
    {
        string tmp = "{\"" + nickname + "\":\"" + UID + "\"}";
        return tmp;
    }

    public IDictionary<string, object> StringToIDictionary(string nickname, string UID)
    {
        IDictionary<string, object> tmp = new Dictionary<string, object>();
        tmp.Add(nickname, UID);
        return tmp;
    }


    public void AcceptFriendClicked()
    {

    }

    public void GetMyRequestFriendList()
    {
        DatabaseManager.instance.reference.GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                DataSnapshot dataSnapshot = (DataSnapshot)snapshot
                                                                .Child(AuthManager.instance.GetAuthUID())
                                                                .Child(DBFriend.Friend)
                                                                .Child(DBFriend.FriendRequests);
                IDictionary id = (IDictionary)dataSnapshot.Value;
                foreach (var tmp in id.Keys)
                {
                    Debug.Log((string)tmp);
                    string a = id[tmp].ToString();
                    Debug.Log(a);
                }
            }
            else
            {
                Debug.Log("데이터 가져오기 실패");
            }
        });

    }






    public void ReceiveFriendRequest()
    {
        //내가 친구요청을 받으면 받은 아이디를 내 FriendList에 Nickname, UID넣어주고 



        DatabaseManager.instance.dbReference
            .Child("UserInfo")
            .Child(requestFriendUID/*상대방 UID넣어주고*/)
            .Child(DBFriend.Friend/*Friend*/)
            .Child(DBFriend.FriendRequests /*DB Friend Request List에 넣어주기*/)
            .Child(DatabaseManager.instance.dbData.DisplayNickname)
            .RemoveValueAsync();


        DatabaseManager.instance.dbReference
             .Child("UserInfo")
             .Child(requestFriendUID/*상대방 UID넣어주고*/)
             .Child(DBFriend.Friend/*Friend*/)
             .Child(DBFriend.FriendRequests /*DB Friend Request List에 넣어주기*/)
             .Child(DatabaseManager.instance.dbData.DisplayNickname)
             .RemoveValueAsync();

        DatabaseManager.instance.dbReference
             .Child("UserInfo")
             .Child(requestFriendUID/*상대방 UID넣어주고*/)
             .Child(DBFriend.Friend/*Friend*/)
             .Child(DBFriend.FriendRequests /*DB Friend Request List에 넣어주기*/)
             .Child(DatabaseManager.instance.dbData.DisplayNickname)
             .RemoveValueAsync();
        //.UpdateChildrenAsync(StringToIDictionary(DatabaseManager.instance.dbData.DisplayNickname, AuthManager.instance.GetAuthUID())); /*내정보*/

    }


}
