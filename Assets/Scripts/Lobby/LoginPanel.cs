
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System.Collections;

public class LoginPanel : MonoBehaviour
{
    public GameObject googleLoginBtn;

    //https://geukggom.tistory.com/155

    void Start()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            // ���� �÷��� ���񽺿� �α��εǾ� ���� ������ �α��� �г��� ���
            if (IsAuthenticated())
                OnLoginButtonClicked();
            else
                googleLoginBtn.SetActive(true);

        }
    }

    public void OnLoginButtonClicked()
    {
        //string playerName = playerNameInput.text;

        //if (playerName == "")
        //{
        //    LobbyManager.instance.ShowError("Invalid Player Name");
        //    return;
        //}

        //PhotonNetwork.LocalPlayer.NickName = playerName;
        PhotonNetwork.ConnectUsingSettings();
    }

    // ���� �÷��� ���񽺿� �α����� �Ǿ��ִ��� Ȯ���ϴ� �Լ�

    public static bool IsAuthenticated()
    {
        return Social.localUser.authenticated;
    }

}
