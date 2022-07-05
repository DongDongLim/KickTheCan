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
    bool isFinishRequestListGetData;
    bool isFinishFriendListGetData ;
    

    //UI


    [SerializeField] GameObject requestFriendprefab;
    [SerializeField] GameObject RequestContent;

    [SerializeField] GameObject friendListprefab;
    [SerializeField] GameObject friendListContent;


    [SerializeField] GameObject friendPanel;
    [SerializeField] GameObject requestPanel;

    bool IsFirstOpenFriendPanel = true;


    DatabaseReference reqeustListReference;
    DatabaseReference friendListReference;



    void Awake()
    {
        dbFriend = new DBFriend();
        isFinishRequestListGetData = false;
        isFinishFriendListGetData = false;
        reqeustListReference = DatabaseManager.instance.reference;
        friendListReference = DatabaseManager.instance.reference;
    }



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
            Debug.Log("없는사람");
        }

    }

    #endregion


    #region Get Friend data in Firebase  

    public void FriendDataGet() // 처음 로그인 되었을때 실행해줘야 하는 함수
    {
        if(IsFirstOpenFriendPanel)
        {
            StartCoroutine("FriendListAddContent");

            StartCoroutine("RequestListAddContent");



            DatabaseManager.instance.dbReference
                    .Child("UserInfo")
                    .Child(AuthManager.instance.GetAuthUID())
                    .Child(DBFriend.Friend/*Friend*/)
                    .Child(DBFriend.FriendLists /*DB Friend Request List에 넣어주기*/).ChildAdded += friendTest;

            DatabaseManager.instance.dbReference
                    .Child("UserInfo")
                    .Child(AuthManager.instance.GetAuthUID())
                    .Child(DBFriend.Friend/*Friend*/)
                    .Child(DBFriend.FriendRequests /*DB Friend Request List에 넣어주기*/).ChildAdded += RequestTest;


            IsFirstOpenFriendPanel = false;
        }
    }

    private void friendTest(object sender, ChildChangedEventArgs e)
    {
        StartCoroutine("FriendListAddContent");
        Debug.Log("상대방, 내가 수락함");

    }
    private void RequestTest(object sender, ChildChangedEventArgs e)
    {
        StartCoroutine("RequestListAddContent");
        Debug.Log("친구요청옴");

    }

    IEnumerator RequestListAddContent()
    {
        Debug.Log("코루틴 시작");


        //데이터 가져오기
        requestNickname = null;
        reqeustListReference.GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;

                DataSnapshot dataSnapshotRequestList = (DataSnapshot)snapshot
                                                                .Child(AuthManager.instance.GetCurrentUID())
                                                                .Child(DBFriend.Friend)
                                                                .Child(DBFriend.FriendRequests);
                requestNickname = (IDictionary)dataSnapshotRequestList.Value;
                Debug.Log("데이터 성공적으로 가져옴");
            }
            else
            {
                Debug.Log("데이터 가져오기 실패");
            }
            isFinishRequestListGetData = true;



        });


        while (!isFinishRequestListGetData)
        {
            yield return null;
            Debug.Log("친구요청 목록 데이터 수집중!!!");
        }

        //친구 목록에 추가
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


        isFinishRequestListGetData = false;
        yield return null;

    }



    IEnumerator FriendListAddContent() //이걸 처음 데이터 가져올때로 바꾸고
    {
        Debug.Log("코루틴 시작");

        friendNickname = null;
        friendListReference.GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;

                DataSnapshot dataSnapshotFriendList = (DataSnapshot)snapshot
                                .Child(AuthManager.instance.GetCurrentUID())
                                .Child(DBFriend.Friend)
                                .Child(DBFriend.FriendLists);


                friendNickname = (IDictionary)dataSnapshotFriendList.Value;

                Debug.Log("데이터 성공적으로 가져옴");
            }
            else
            {
                Debug.Log("데이터 가져오기 실패");
            }
            isFinishFriendListGetData = true;


        });

        while (!isFinishFriendListGetData)
        {
            yield return null;
            Debug.Log("친구 목록 데이터 수집중!!!");
        }

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

        isFinishFriendListGetData = false;
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
        requestPanel.SetActive(true);
    }

    #endregion


}
