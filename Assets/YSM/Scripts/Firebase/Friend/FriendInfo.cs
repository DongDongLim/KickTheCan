using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FriendInfo : MonoBehaviour
{

    string UID;
    [SerializeField] private Text friendName;
    [SerializeField] private Text score;
    IDictionary id;
    FriendListEntry tmpEntry;

    [SerializeField] private GameObject chatPanel;

    public void SetUID(ref FriendListEntry entry)
    {
        if (UID == entry.UID)
            return;

        this.UID = entry.UID;
        Debug.Log(UID);
        tmpEntry = entry;
        chatPanel.GetComponent<FriendChat>().friendUID = UID;

        friendName.text = entry.GetName();
        StartCoroutine("GetUserInfo");
    }

    IEnumerator GetUserInfo()
    {
        bool isFinish = false;

        DatabaseManager.instance.reference = FirebaseDatabase.DefaultInstance.GetReference("UserInfo");

        DatabaseManager.instance.reference.GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                DataSnapshot dataSnapshot = snapshot.Child(UID);
                id = (IDictionary)dataSnapshot.Value;
            }
            else
            {
                Debug.Log("데이터 가져오기 실패");
            }
            isFinish = true;
            Debug.Log("끝남");
        });
        
        while (!isFinish){ yield return null; }
        Debug.Log(id["Score"].ToString());
        score.text = id["Score"].ToString();
        yield return null;


    }

    public void FriendInfoCloseBtnClicked()
    {
        this.gameObject.SetActive(false);
    }


    public void FriendDeleteBtnClicked()
    {
        //Destroy(tmpEntry.gameObject); FriendInfoCloseBtnClicked();

        DatabaseManager.instance.dbReference
             .Child("UserInfo")
             .Child(AuthManager.instance.GetCurrentUID()/*내 UID에 있는거 지우고*/)
             .Child(DBFriend.Friend/*Friend*/)
             .Child(DBFriend.FriendLists /*DB Friend List에 있는 데이터 지워주기*/)
             .Child(friendName.text/*지울 데이터*/)
             .RemoveValueAsync();

        DatabaseManager.instance.dbReference
             .Child("UserInfo")
             .Child(UID/*내 UID에 있는거 지우고*/)
             .Child(DBFriend.Friend/*Friend*/)
             .Child(DBFriend.FriendLists /*DB Friend List에 있는 데이터 지워주기*/)
             .Child(DatabaseManager.instance.dbData.DisplayNickname/*지울 데이터*/)
             .RemoveValueAsync();

        //삭제 코드 추가

    }

    public void FriendChatBtnClicked()
    {
        chatPanel.SetActive(true);
    }

}
