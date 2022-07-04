using Firebase;
using Firebase.Database;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DatabaseManager : MonoBehaviour
{
    [SerializeField]
    Text testTxt;

    static public DatabaseManager instance { get; private set; }

    public DatabaseReference dbReference;
    public DatabaseReference reference;

    public DBData dbData;
    public DBData dbDataGoogle;

    public bool CanLogout;


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

        string json = JsonUtility.ToJson(mydata);
        dbReference.Child("UserInfo").Child(AuthManager.instance.GetAuthUID()).SetRawJsonValueAsync(json);
        //이 함수 사용하면 덮어 씌워짐
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
                dbData = new DBData(id["Email"].ToString(), id["DisplayNickname"].ToString(), id["Score"].ToString());
                //SetUserDataInDataBase(dbData);
                PhotonNetwork.LocalPlayer.NickName = dbData.DisplayNickname;
                PhotonNetwork.ConnectUsingSettings();
            }
            else
            {
                Debug.Log("데이터 가져오기 실패");
            }
        });

    }
#if UNITY_ANDROID
    // GoogleFirebaseLogin / DH

    public IEnumerator MyNickNameCheck()
    {
        bool isFinish = true;
        bool isNickName = false;
        testTxt.text = "0";
        reference.GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                DataSnapshot dataSnapshot = (DataSnapshot)snapshot.Child(AuthManager.instance.GetAuthUID());
                testTxt.text = dataSnapshot.ToString();
                IDictionary id = (IDictionary)dataSnapshot.Value;

                
                //testTxt.text = id.Count.ToString();
                //dbDataGoogle = new DBData(id["Email"].ToString(), id["DisplayNickname"].ToString(), id["Score"].ToString());
                //SetUserDataInDataBase(dbData);
                if (id == null || id.Count == 0)
                    isNickName = false;
                else
                    isNickName = true;
                testTxt.text = "4";
            }
            else
            {
                Debug.Log("데이터 가져오기 실패");
            }
            isFinish = false;
            testTxt.text = "2";
        });
        while (isFinish)
            yield return null;
        AuthManager.instance.isNickName = isNickName;
        testTxt.text = "3";
    }
#endif

    //public void test()
    //{
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

}
