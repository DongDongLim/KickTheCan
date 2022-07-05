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


    private void Start()
    {
        instance = this;
    }


    public void SetEventListener()
    {

        FirebaseDatabase.DefaultInstance
            .GetReference("UserInfo")
            .Child(AuthManager.instance.GetAuthUID())
            .Child(DBFriend.Friend)
            .Child(DBFriend.FriendLists)
            .ChildAdded += friendTest;

        FirebaseDatabase.DefaultInstance
            .GetReference("UserInfo")
            .Child(AuthManager.instance.GetAuthUID())
            .Child(DBFriend.Friend)
            .Child(DBFriend.FriendRequests)
            .ChildAdded += RequestTest;
    }


    private void friendTest(object sender, ChildChangedEventArgs e)
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
    }


    private void RequestTest(object sender, ChildChangedEventArgs e)
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





}
