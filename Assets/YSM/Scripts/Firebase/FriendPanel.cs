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


    
    private IDictionary friendNickname;
    private IDictionary requestNickname;
    bool FinishGetAllFriendData;
    bool FinishGetRequestFriendData;
    

    //UI


    [SerializeField] GameObject requestFriendprefab;
    [SerializeField] GameObject RequestContent;

    [SerializeField] GameObject friendListprefab;
    [SerializeField] GameObject friendListContent;


    [SerializeField] GameObject friendPanel;
    [SerializeField] GameObject requestPanel;

    //bool IsFirstOpenFriendPanel = true;

    void Start()
    {
        dbFriend = new DBFriend();
        FinishGetAllFriendData = false;

        DatabaseManager.instance.reference.ValueChanged += ReceiveMessage;
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
            Debug.Log("친구추가");
            RequestFriend();
        }
        else
        {
            Debug.Log("없는사람");
        }

    }

    #endregion
    //public string DictionaryToJson(string nickname, string UID)
    //{
    //    string tmp = "{\"" + nickname + "\":\"" + UID + "\"}";
    //    return tmp;
    //}

    public IDictionary<string, object> StringToIDictionary(string nickname, string UID)
    {
        IDictionary<string, object> tmp = new Dictionary<string, object>();
        tmp.Add(nickname, UID);
        return tmp;
    }


    #region Get Friend data in Firebase  

    public void FriendDataGet() // 처음 로그인 되었을때 실행해줘야 하는 함수
    {

            StartCoroutine("FriendRequestListAddContent");
            GetMyRequestFriendList();
            //IsFirstOpenFriendPanel = false;

    }

    public void GetMyRequestFriendList()
    {
        requestNickname = null;
        friendNickname = null;
        DatabaseManager.instance.reference.GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;

                DataSnapshot dataSnapshotFriendList = (DataSnapshot)snapshot
                                .Child(AuthManager.instance.GetAuthUID())
                                .Child(DBFriend.Friend)
                                .Child(DBFriend.FriendLists);

                DataSnapshot dataSnapshotRequestList = (DataSnapshot)snapshot
                                                                .Child(AuthManager.instance.GetAuthUID())
                                                                .Child(DBFriend.Friend)
                                                                .Child(DBFriend.FriendRequests);

                
                friendNickname = (IDictionary)dataSnapshotFriendList.Value;
                requestNickname = (IDictionary)dataSnapshotRequestList.Value;


                Debug.Log("데이터 성공적으로 가져옴");
            }
            else
            {
                Debug.Log("데이터 가져오기 실패");
            }
            FinishGetAllFriendData = true;


        });

    }

    public void GetMyRequestList()
    {
        requestNickname = null;
        DatabaseManager.instance.reference.GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;

                DataSnapshot dataSnapshotRequestList = (DataSnapshot)snapshot
                                                                .Child(AuthManager.instance.GetAuthUID())
                                                                .Child(DBFriend.Friend)
                                                                .Child(DBFriend.FriendRequests);
                requestNickname = (IDictionary)dataSnapshotRequestList.Value;


                Debug.Log("데이터 성공적으로 가져옴");
            }
            else
            {
                Debug.Log("데이터 가져오기 실패");
            }
            FinishGetRequestFriendData = true;


        });

    }

    IEnumerator RequestListAddContent()
    {
        Debug.Log("코루틴 시작");
        while (!FinishGetRequestFriendData)
        {
            yield return new WaitForSeconds(0.01f);
            Debug.Log("데이터 수집중!!!");
        }

        if (requestNickname != null)
        {
            Transform[] tmp = RequestContent.GetComponentsInChildren<Transform>();
            for (int i =1; i < tmp.Length; i++)
            {
                Destroy(tmp[i].gameObject);
            }

            foreach (string name in requestNickname.Keys)
            {
                GameObject entry = Instantiate(requestFriendprefab);
                entry.GetComponent<FriendRequestEntry>().SetData(
                    name.ToString(),
                    requestNickname[name].ToString()
                    );
                entry.transform.localScale = Vector3.one;
                entry.transform.SetParent(RequestContent.transform);
            }
        }


        FinishGetRequestFriendData = false;

        yield return null;
    }



    IEnumerator FriendRequestListAddContent()
    {
        Debug.Log("코루틴 시작");
        while (!FinishGetAllFriendData)
        {
            yield return new WaitForSeconds(0.01f);
            Debug.Log("데이터 수집중!!!");
        }
        
        //if (requestNickname != null)
        //{
        //    foreach (string name in requestNickname.Keys)
        //    {
        //        GameObject entry = Instantiate(requestFriendprefab);
        //        entry.GetComponent<FriendRequestEntry>().SetData(
        //            name.ToString(),
        //            requestNickname[name].ToString()
        //            );
        //        entry.transform.localScale = Vector3.one;
        //        entry.transform.SetParent(RequestContent.transform);
        //    }
        //}

        if (friendNickname != null)
        {
            Transform[] tmp = friendListContent.GetComponentsInChildren<Transform>();
            for (int i = 1; i < tmp.Length; i++)
            {
                Destroy(tmp[i].gameObject);
            }

            foreach (string name in friendNickname.Keys)
            {
                GameObject entry = Instantiate(friendListprefab);
                entry.GetComponent<FriendListEntry>().SetData(
                    name.ToString(),
                    friendNickname[name].ToString()
                    );
                entry.transform.localScale = Vector3.one;
                entry.transform.SetParent(friendListContent.transform);
            }
        }



        FinishGetAllFriendData = false;
        yield return null;
    }


#endregion


    #region Friend Receive , Cansel
    public void ReceiveFriendRequest(FriendRequestEntry entry)
    {
        Debug.Log(entry.GetRequestFriendName());
        //내가 친구요청을 받으면 받은 아이디를 내 FriendList에 Nickname, UID넣어주고 
        DatabaseManager.instance.dbReference
            .Child("UserInfo")
            .Child(AuthManager.instance.GetAuthUID()/*내 UID에*/)
            .Child(DBFriend.Friend/*Friend*/)
            .Child(DBFriend.FriendLists /*DB Friend List에 넣어주기*/)
            .UpdateChildrenAsync(StringToIDictionary(entry.GetRequestFriendName() ,entry.GetUID())/*넣을 데이터*/);

        //내가 받은 친구요청을 상대방에게도 추가해주고
        DatabaseManager.instance.dbReference
             .Child("UserInfo")
             .Child(entry.GetUID()/*상대방 UID넣어주고*/)
             .Child(DBFriend.Friend/*Friend*/)
             .Child(DBFriend.FriendLists /*DB Friend List에 넣어주기*/)
             .UpdateChildrenAsync(StringToIDictionary(DatabaseManager.instance.dbData.DisplayNickname, AuthManager.instance.GetAuthUID())/*내정보*/);

        //내 request요청에 있는거 지워주고
        DatabaseManager.instance.dbReference
             .Child("UserInfo")
             .Child(AuthManager.instance.GetAuthUID()/*내 UID에 있는거 지우고*/)
             .Child(DBFriend.Friend/*Friend*/)
             .Child(DBFriend.FriendRequests /*DB Friend Request List에 있는 데이터 지워주기*/)
             .Child(entry.GetRequestFriendName()/*지울 데이터*/)
             .RemoveValueAsync();



        GameObject friendprefebEntry = Instantiate(friendListprefab);
        friendprefebEntry.GetComponent<FriendListEntry>().SetData(
            entry.GetRequestFriendName(),
            entry.GetUID()
            );
        friendprefebEntry.transform.localScale = Vector3.one;
        friendprefebEntry.transform.SetParent(friendListContent.transform);



        Destroy(entry.gameObject);
    }
    
    public void CancelFriendRequest(FriendRequestEntry entry)
    {
        Debug.Log(entry.GetRequestFriendName());
        DatabaseManager.instance.dbReference
             .Child("UserInfo")
             .Child(AuthManager.instance.GetAuthUID()/*내 UID에 있는거 지우고*/)
             .Child(DBFriend.Friend/*Friend*/)
             .Child(DBFriend.FriendRequests /*DB Friend Request List에 있는 데이터 지워주기*/)
             .Child(entry.GetRequestFriendName()/*지울 데이터*/)
             .RemoveValueAsync();
        Destroy(entry.gameObject);
    }

    public void OpenFriendInfo(FriendListEntry entry)
    {
        Debug.Log("OpenClicked");
        //TODO:열었을때 UI 활성화 시켜야함 
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
        StartCoroutine("RequestListAddContent");
        GetMyRequestList();
        requestPanel.SetActive(true);
    }

    #endregion

    
    private void ReceiveMessage(object sender , ValueChangedEventArgs e)
    {

    }
}
