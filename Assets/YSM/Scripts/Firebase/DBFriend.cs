
using System.Collections.Generic;
using UnityEngine;

public class DBFriend
{
    public static string Friend = "Friend";
    public static string FriendRequests = "FriendRequests";
    public static string FriendLists = "FriendList";


    private Dictionary<string, string> FriendList;
    private Dictionary<string, string> FriendRequestList;

    public DBFriend()
    {
        FriendList = new Dictionary<string, string>();
        FriendRequestList = new Dictionary<string, string>();
    }

    ~DBFriend()
    {
        FriendList.Clear();
    }

    public Dictionary<string, string> GetFriendData()
    {
        return FriendList;
    }

    public void AddFriend(string nickname, string UID)
    {
        FriendList.Add(nickname, UID);
    }
        
    public void AddFriendRequestList(string nickname, string UID)
    {
        FriendRequestList.Add(nickname, UID);
        foreach(var data in FriendRequestList.Keys)
        {
            string a =  FriendRequestList[data.ToString()];

            Debug.Log(data.ToString() + a);
        }
    }




}
