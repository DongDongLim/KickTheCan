
using System.Collections.Generic;

public class DBFriendList
{

    private Dictionary<string, string> FriendList;

    public DBFriendList()
    {
        FriendList = new Dictionary<string, string>();
    }

    ~DBFriendList()
    {
        FriendList.Clear();
    }

    public Dictionary<string ,string> GetFriendData()
    {
        return FriendList;
    }

    public void AddFriend(string nickname, string UID)
    {
        FriendList.Add(nickname, UID);
    }




}
