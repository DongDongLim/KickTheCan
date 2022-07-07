
using System.Collections.Generic;
using UnityEngine;

public class DBFriend
{
    public static string Friend = "Friend";
    public static string FriendRequests = "FriendRequests";
    public static string FriendLists = "FriendList";

}

public class DBFriendChat
{
    public string name;
    public string message;
    public DBFriendChat(string name, string message)
    {
        this.name = name;
        this.message = message;
    }
}