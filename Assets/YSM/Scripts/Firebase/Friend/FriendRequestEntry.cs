using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FriendRequestEntry : MonoBehaviour
{
    [SerializeField] private Text RequestFriendName;
    [SerializeField] private Button Yesbtn;
    [SerializeField] private Button Nobtn;
    private FriendPanel friendPanel;

    private string UID;

    private void Start()
    {
        friendPanel=FindObjectOfType<FriendPanel>();
        Yesbtn.onClick.AddListener(() 
            => { friendPanel.ReceiveFriendRequest(this); });
        Nobtn.onClick.AddListener(() 
            => { friendPanel.CancelFriendRequest(this); });
    }

    public void SetData(string name , string UID)
    {
        this.RequestFriendName.text = name;
        this.UID = UID;
    }



    public string GetUID()
    {
        return UID;
    }

    public string GetRequestFriendName()
    {
        return RequestFriendName.text;
    }
    


}
