using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FriendListEntry : MonoBehaviour
{
    [SerializeField] private Text friendName;
    [SerializeField] private Image loginImage;
    [SerializeField] private bool loginState;

    [SerializeField] private Button openBtn;
    private FriendPanel friendPanel;

    private string UID;

    DatabaseReference eachReference;


    private void Start()
    {
        friendPanel = FindObjectOfType<FriendPanel>();
        openBtn.onClick.AddListener(()
            => { friendPanel.OpenFriendInfo(this); });
    }

    public void SetData(string name, string UID)
    {
        this.friendName.text = name;
        this.UID = UID;
        eachReference = FirebaseDatabase.DefaultInstance.GetReference("UserInfo").Child(UID).Child(DBData.KeyIsLogin);
        eachReference.ValueChanged += LoginStateUI;

        StartCoroutine("GetMyData");
    }


    IEnumerator GetMyData()
    {
        bool isFinish = false;
        eachReference.GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                DataSnapshot dataSnapshot = (DataSnapshot)snapshot.Child(UID);
                IDictionary id = (IDictionary)dataSnapshot.Value;
                loginState = (bool)id[DBData.KeyEmail];
            }
            else
            {
                Debug.Log("데이터 가져오기 실패");
            }
            isFinish = true;

        });

        while (!isFinish)
        {
            yield return null;
        }
        LoginStateUI();
    }


    private void LoginStateUI(object sender , ValueChangedEventArgs e)
    {
        DataSnapshot snapshot = e.Snapshot;
        loginState = (bool)snapshot.Value;
        Debug.Log(loginState);
        if (this.gameObject.activeSelf == false)
        {
            return;
        }

        LoginStateUI();
    }
    private void LoginStateUI()
    {
        if(loginState == true)
        {
            //로그인중
            openBtn.interactable = true;
            loginImage.color = Color.green;

        }
        else
        {
            //로그아웃 
            openBtn.interactable = false;
            loginImage.color = Color.black;

        }
    }

    public string GetUID()
    {
        return UID;
    }

    public string GetRequestFriendName()
    {
        return friendName.text;
    }
    private void OnDestroy()
    {
        eachReference.ValueChanged -= LoginStateUI;
    }
}
