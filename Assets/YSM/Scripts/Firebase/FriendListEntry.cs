using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FriendListEntry : MonoBehaviour
{
    [SerializeField] private Text friendName;
    [SerializeField] private Button openBtn;
    private FriendPanel friendPanel;

    private string UID;

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
    }



    public string GetUID()
    {
        return UID;
    }

    public string GetRequestFriendName()
    {
        return friendName.text;
    }
}
