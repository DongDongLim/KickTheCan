using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FriendChat : MonoBehaviour
{

    [SerializeField] private InputField messageField;

    IDictionary chatData;

    private string _friendUID;
    public string friendUID
    {
        private get
        {
            return _friendUID;
        }
        set
        {
            Debug.Log("알ㄹㄹㄹㄹㄹㄹㄹㄹㄹㄹㄹㄹㄹㄹㄹㄹㄹ라ㅏㅏㅏㅏㅏㅏㅏㅏㅏㅏㅏㅏㅏㅏㅏㅏ");
            _friendUID = value;
            StartCoroutine("FriendMessageGet");
        }
    }


    IEnumerator FriendMessageGet()
    {
        bool isFinish = false;    

        DatabaseManager.instance.chatReference = FirebaseDatabase.DefaultInstance.GetReference("FriendChat");
        DatabaseManager.instance.chatReference.GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {//,AuthManager.instance.GetAuthUID()
                DataSnapshot snapshot = task.Result;
                DataSnapshot dataSnapshot = snapshot.Child(FuncTool.CompareStrings(_friendUID, "123"));
                chatData = (IDictionary)dataSnapshot.Value;
            }
            else
            {
                Debug.Log("데이터 가져오기 실패");
            }
            isFinish = true;
            Debug.Log("끝남");
        });

        while (!isFinish) { yield return null; }
        
        //가져온 데이터 처리



        yield return null;

    }  


    public void FriendMessageSendClicked()
    {
        Debug.Log("메세지 보내기 성공!!!!!!!!!!!!");
        if (messageField.text == "")
            return;


        //DatabaseManager.instance.dbReference
        //    .Child("UserInfo")
        //    .Child("18JdkaumF6RSztkhYmUx3yQ7VQ02"/*상대방 UID넣어주고*/)
        //    .Child(DBFriend.Friend/*Friend*/)
        //    .Child(DBFriend.FriendLists /*DB Friend List에 넣어주기*/)
        //    .Child(DatabaseManager.instance.dbData.DisplayNickname)
        //    .Child("Chat");
        ////  .UpdateChildrenAsync(FuncTool.ConvertToIDictionary(DatabaseManager.instance.dbData.DisplayNickname, AuthManager.instance.GetCurrentUID())/*내정보*/);


        DBData dbData = new DBData("Aaa", "bbb", 123, true);
        string json = JsonUtility.ToJson(dbData);
        FirebaseDatabase.DefaultInstance
            .GetReference("FriendChat")
            .Child(AuthManager.instance.GetAuthUID()+"18JdkaumF6RSztkhYmUx3yQ7VQ02")
            .SetRawJsonValueAsync(json);

        //    //        dbReference.Child("UserInfo").Child(AuthManager.instance.GetAuthUID()).SetRawJsonValueAsync(json);
        //    //.Child(FuncTool.CompareStrings(_friendUID, AuthManager.instance.GetAuthUID())/* chat UID 만들고*/)
        //    //.Child(ServerValue.Timestamp.ToString()/*Friend*/)
        //    //.UpdateChildrenAsync(FuncTool.ConvertToIDictionary(DatabaseManager.instance.dbData.DisplayNickname, messageField.text)/*넣을 데이터*/);
        //messageField.text = "";
    }

}
