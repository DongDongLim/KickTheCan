using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DatabaseManager : MonoBehaviour
{
    public InputField id;
    public InputField data;


    private string userID;
    private DatabaseReference dbReference;

    private void Start()
    {
        userID = SystemInfo.deviceUniqueIdentifier;
        dbReference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public void CreateUser()
    {
        Debug.Log(dbReference);
        Debug.Log(userID);


        myData newUser = new myData(id.text, data.text);
        string json = JsonUtility.ToJson(newUser);

        dbReference.Child("user").Child(userID).SetRawJsonValueAsync(json);

        Debug.Log("createuserFuncton");
    }


}
