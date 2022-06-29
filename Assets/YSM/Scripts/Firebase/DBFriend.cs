
using System.Collections.Generic;

public class DBFriend
{
    public static string Friend = "Friend";
    public static string FriendRequests = "FriendRequests";
    public static string FriendLists = "FriendList";


    private Dictionary<string, string> FriendList;

    public DBFriend()
    {
        FriendList = new Dictionary<string, string>();
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




    //public void RequestFriend()
    //{

    //}

}
