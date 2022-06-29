using Firebase;
using Firebase.Database;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;


public class DatabaseManager : MonoBehaviour
{

    private DatabaseReference dbReference;
    public DatabaseReference reference { get; set; }
    static public DatabaseManager instance { get; private set; }


    public DBData data;


    public bool CanLogout;
    public GameObject test112;
    public bool test11 = true;

    void Start()
    {
        instance = this;
        dbReference = FirebaseDatabase.DefaultInstance.RootReference;
        
        reference = FirebaseDatabase.DefaultInstance.GetReference("UserInfo");


        // 사용하고자 하는 데이터를 reference가 가리킴
        // 여기선 rank 데이터 셋에 접근
    }

    public void SetUserDataInDataBase(DBData mydata)
    {
        DBFriendList test = new DBFriendList();
        test.AddFriend("sksksk", "slslsl");
        string json = JsonUtility.ToJson(mydata);
        string json2 = JsonConvert.SerializeObject(test);


        dbReference.Child("UserInfo").Child(AuthManager.instance.GetAuthUID()).SetRawJsonValueAsync(json);
        dbReference.Child("UserInfo").Child(AuthManager.instance.GetAuthUID()).Child("FriendList").SetRawJsonValueAsync(json2);


    }




    public void GetMyData()
    {
        reference.GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            { 
                DataSnapshot snapshot = task.Result;
                DataSnapshot dataSnapshot = (DataSnapshot)snapshot.Child(AuthManager.instance.GetAuthUID());
                IDictionary id = (IDictionary)dataSnapshot.Value;
                data = new DBData(id["Email"].ToString(), id["DisplayNickname"].ToString(), id["Score"].ToString());
                SetUserDataInDataBase(data);

                PhotonNetwork.LocalPlayer.NickName = data.DisplayNickname;
                PhotonNetwork.ConnectUsingSettings();
            }
            else
            {
                Debug.Log("데이터 가져오기 실패");
            }
        });
        
    }

    public void test()
    {
        data.IsLoggingIn = "false";
        string json = JsonUtility.ToJson(data);


        Dictionary<string, object> update = new Dictionary<string, object>();
        update["IsLoggingIn"] = "false";
        dbReference.Child("UserInfo").Child(AuthManager.instance.GetAuthUID()).UpdateChildrenAsync(update).ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                CanLogout = true;
                Debug.Log("굿/");
                Application.Quit();
            }
            else
            {
                CanLogout = false;
            }
        });
        Debug.Log("끝남 함수");
    }

    
    //void OnApplicationPause()
    //{

    //    //test112.gameObject.SetActive(test11);
    //    //test11 = !test11;
    //    //if (!CanLogout)
    //    //{
    //    //    Application.CancelQuit();
    //    //    test();
    //    //}
    //    data.IsLoggingIn = "false";
    //    string json = JsonUtility.ToJson(data);


    //    Dictionary<string, object> update = new Dictionary<string, object>();
    //    update["IsLoggingIn"] = "false";
    //    dbReference.Child("UserInfo").Child(AuthManager.instance.GetAuthUID()).UpdateChildrenAsync(update).ContinueWith(task =>
    //    {
    //        if (task.IsCompleted)
    //        {
    //            CanLogout = true;
    //            Debug.Log("굿/");
    //            Application.Quit();
    //        }
    //        else
    //        {
    //            CanLogout = false;
    //        }
    //    });
    //    Debug.Log("끝남 함수");
    //}


    void FriendRequest()
    {

    }


}
