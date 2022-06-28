using Firebase;
using Firebase.Database;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DatabaseManager : MonoBehaviour
{



    private DatabaseReference dbReference;
    public DatabaseReference reference { get; set; }
    static public DatabaseManager instance { get; private set; }


    public myData data;


    void Start()
    {
        instance = this;
        dbReference = FirebaseDatabase.DefaultInstance.RootReference;
        reference = FirebaseDatabase.DefaultInstance.GetReference("UserInfo");
        // 사용하고자 하는 데이터를 reference가 가리킴
        // 여기선 rank 데이터 셋에 접근
    }

    public void SetUserDataInDataBase(myData mydata)
    {
        string json = JsonUtility.ToJson(mydata);
        dbReference.Child("UserInfo").Child(AuthManager.instance.GetAuthUID()).SetRawJsonValueAsync(json);
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
                data = new myData(id["Email"].ToString(), id["DisplayNickname"].ToString(), id["Score"].ToString());
                PhotonNetwork.LocalPlayer.NickName = data.DisplayNickname;
                PhotonNetwork.ConnectUsingSettings();
            }
            else
            {
                Debug.Log("데이터 가져오기 실패");
            }
        });
        
    }
   
}
