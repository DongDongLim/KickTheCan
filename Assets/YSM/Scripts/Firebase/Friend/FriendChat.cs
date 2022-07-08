using Firebase.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FriendChat : MonoBehaviour
{

    [SerializeField] private InputField messageField;
    [SerializeField] private GameObject friendChatPrefab;
    [SerializeField] private GameObject friendChatContentPanel;
    [SerializeField] private ScrollRect test;


    DataSnapshot dataSnapshot;

    bool dataGetSuccese;


    private void Awake()
    {
        test = GetComponent<ScrollRect>();
    }

    private string _friendUID;
    public string friendUID
    {
        private get
        {
            return _friendUID;

        }
        set
        {
            int i = 0;
            _friendUID = value;
            FriendManager.instance.SetCurrentPanel(_friendUID);
            test.content = FriendManager.instance.CurContentPanel.GetComponent<RectTransform>();
            test.content.anchoredPosition = Vector3.zero;
            test.content.position = Vector3.zero;
            test.content.transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 0);
            test.content.transform.localScale = Vector3.one;
            test.content.offsetMin = new Vector2(0, test.content.offsetMin.y);


        }
    }



    public void FriendMessageSendClicked()
    {
        Debug.Log("메세지 보내기 성공!!!!!!!!!!!!");
        if (messageField.text == "")
            return;


        string key = DatabaseManager.instance.chatReference.Push().Key;

        Dictionary<string, object> msgDic = new Dictionary<string, object>();

        msgDic.Add("username", DatabaseManager.instance.dbData.DisplayNickname);
        msgDic.Add("message", messageField.text);
        msgDic.Add("timestamp", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));
        msgDic.Add("parent", FuncTool.CompareStrings(AuthManager.instance.GetAuthUID(), _friendUID));

        Dictionary<string, object> updateMsg = new Dictionary<string, object>();
        updateMsg.Add(key, msgDic);

        DatabaseManager.instance.chatReference.Child(FuncTool.CompareStrings(AuthManager.instance.GetAuthUID(), _friendUID)).UpdateChildrenAsync(updateMsg);
        messageField.text = "";
    }


    public void FriendChatOpenClicked()
    {
        this.gameObject.SetActive(true);
    }

    public void FriendChatCloseBtnClicked()
    {
        this.gameObject.SetActive(false);
    }

}
