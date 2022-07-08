using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendManager : MonoBehaviour
{

    public static FriendManager instance { get; private set; }

    [SerializeField] GameObject friendListprefab;
    [SerializeField] GameObject friendContent;


    [SerializeField] GameObject requestFriendprefab;
    [SerializeField] GameObject RequestContent;





    Dictionary<string, GameObject> myfriendDictionary;





    [SerializeField] GameObject friendChatContentPanelParent;
    [SerializeField] GameObject friendChatPrefab;
    [SerializeField] GameObject friendChatContentPanelPrefab;

    public GameObject CurContentPanel;
    Dictionary<string, GameObject> friendChatcontentPanel;

    [SerializeField] FriendInfo friendInfoPanel;





    private void Start()
    {
        instance = this;
        myfriendDictionary = new Dictionary<string, GameObject>();
        friendChatcontentPanel = new Dictionary<string, GameObject>();
    }

    public void SetEventListener()
    {

        FirebaseDatabase.DefaultInstance
            .GetReference("UserInfo")
            .Child(AuthManager.instance.GetAuthUID())
            .Child(DBFriend.Friend)
            .Child(DBFriend.FriendLists)
            .ChildAdded += FriendListAdd;



        FirebaseDatabase.DefaultInstance
            .GetReference("UserInfo")
            .Child(AuthManager.instance.GetAuthUID())
            .Child(DBFriend.Friend)
            .Child(DBFriend.FriendRequests)
            .ChildAdded += RequestListAdd;

        FirebaseDatabase.DefaultInstance
            .GetReference("UserInfo")
            .Child(AuthManager.instance.GetAuthUID())
            .Child(DBFriend.Friend)
            .Child(DBFriend.FriendLists)
            .ChildRemoved += FriendRemove;

    }


    private void FriendListAdd(object sender, ChildChangedEventArgs e)
    {
        Debug.Log("내 친구 데이터 가져옴");
        Debug.Log(e.Snapshot.Key);
        Debug.Log(e.Snapshot.Value.ToString());


        GameObject entry = Instantiate(friendListprefab);
        entry.GetComponent<FriendListEntry>().SetData(
            e.Snapshot.Key,
            e.Snapshot.Value.ToString()
            );

        entry.transform.localScale = Vector3.one;
        entry.transform.SetParent(friendContent.transform);
        myfriendDictionary.Add(e.Snapshot.Key, entry);



        GameObject panel = Instantiate(friendChatContentPanelPrefab);
        panel.transform.SetParent(friendChatContentPanelParent.transform);
        friendChatcontentPanel.Add(FuncTool.CompareStrings(e.Snapshot.Value.ToString(), AuthManager.instance.GetAuthUID()), panel);

        //FirebaseDatabase.DefaultInstance
        //    .GetReference("FriendChat")
        //    .Child(FuncTool.CompareStrings(e.Snapshot.Value.ToString(), AuthManager.instance.GetAuthUID()))
        //    .ChildAdded += ChatAdd;



        var childRef =FirebaseDatabase.DefaultInstance
            .GetReference("FriendChat")
            .Child(FuncTool.CompareStrings(e.Snapshot.Value.ToString(), AuthManager.instance.GetAuthUID()));
        childRef.ChildAdded += ChatAdd;

        Debug.Log(FuncTool.CompareStrings(e.Snapshot.Value.ToString(), AuthManager.instance.GetAuthUID()) + "추가됨 친구 추가 채팅");
    }



    private void RequestListAdd(object sender, ChildChangedEventArgs e)
    {
        Debug.Log("친구요청옴");
        Debug.Log(e.Snapshot.Key);
        Debug.Log(e.Snapshot.Value.ToString());
        GameObject entry = Instantiate(requestFriendprefab);
        entry.GetComponent<FriendRequestEntry>().SetData(
            e.Snapshot.Key,
            e.Snapshot.Value.ToString()
            );
        entry.transform.localScale = Vector3.one;
        entry.transform.SetParent(RequestContent.transform);


    }

    private void FriendRemove(object sender, ChildChangedEventArgs e)
    {
        //FirebaseDatabase.DefaultInstance
        //    .GetReference("UserInfo")
        //    .Child(AuthManager.instance.GetAuthUID())
        //    .Child(DBFriend.Friend)
        //    .Child(e.Snapshot.Key)
        //    .ChildRemoved -= FriendRemove;


        Debug.Log("내 친구 삭제됨");
        Debug.Log(e.Snapshot.Key);
        Debug.Log(e.Snapshot.Value.ToString());
        Destroy(myfriendDictionary[e.Snapshot.Key]);


        DatabaseManager.instance.chatReference = FirebaseDatabase.DefaultInstance.GetReference("FriendChat");
        Debug.Log(FuncTool.CompareStrings(e.Snapshot.Value.ToString(), AuthManager.instance.GetAuthUID()) + "제거됨 친구 추가 채팅");


        var childRef = FirebaseDatabase.DefaultInstance
            .GetReference("FriendChat")
            .Child(FuncTool.CompareStrings(e.Snapshot.Value.ToString(), AuthManager.instance.GetAuthUID()));
            childRef.ChildAdded -= ChatAdd;

        DatabaseManager.instance.chatReference
            .Child(FuncTool.CompareStrings(AuthManager.instance.GetAuthUID(), e.Snapshot.Value.ToString()))
            .RemoveValueAsync();

        //FirebaseDatabase.DefaultInstance
        //    .GetReference("UserInfo")
        //    .Child(AuthManager.instance.GetAuthUID())
        //    .Child(DBFriend.Friend)
        //    .Child(DBFriend.FriendLists)
        //    .ChildAdded -= FriendListAdd;



        if (CurContentPanel == friendChatcontentPanel[FuncTool.CompareStrings(AuthManager.instance.GetAuthUID(), e.Snapshot.Value.ToString())])
        {
            Destroy(CurContentPanel);
            CurContentPanel = null;
        }
        else
        {
            Destroy(friendChatcontentPanel[FuncTool.CompareStrings(AuthManager.instance.GetAuthUID(), e.Snapshot.Value.ToString())]);
        }



        myfriendDictionary.Remove(e.Snapshot.Key);
        friendChatcontentPanel.Remove(FuncTool.CompareStrings(AuthManager.instance.GetAuthUID(), e.Snapshot.Value.ToString()));

        //friendInfoPanel.SetNull(e.Snapshot.Value.ToString());


    }

    public void ChatAdd(object sender, ChildChangedEventArgs e)
    {
        Debug.Log("채팅 가져옴~~");
        GameObject friendChatEntry = Instantiate(friendChatPrefab);
        friendChatEntry.GetComponent<FriendChatEntry>().SetData(
            e.Snapshot.Child("timestamp").Value.ToString(),
            e.Snapshot.Child("username").Value.ToString(),
            e.Snapshot.Child("message").Value.ToString(),
            e.Snapshot.Child("username").Value.ToString() == DatabaseManager.instance.dbData.DisplayNickname
            );
        friendChatEntry.transform.SetParent(friendChatcontentPanel[e.Snapshot.Child("parent").Value.ToString()].transform);
    }

    public void SetCurrentPanel(string friendUID)
    {
        if(CurContentPanel != null)
            CurContentPanel.SetActive(false);
        Debug.Log("세팅함1");
        CurContentPanel = friendChatcontentPanel[FuncTool.CompareStrings(AuthManager.instance.GetAuthUID(), friendUID)];
        Debug.Log("세팅함2");
        CurContentPanel.SetActive(true);
    }
}
