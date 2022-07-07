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
    private void Start()
    {
        instance = this;
        myfriendDictionary = new Dictionary<string, GameObject>();
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
        Debug.Log(myfriendDictionary.Count);
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
        Debug.Log("내 친구 삭제됨");
        Debug.Log(e.Snapshot.Key);
        Debug.Log(e.Snapshot.Value.ToString());

        Destroy(myfriendDictionary[e.Snapshot.Key]);


    }




}
