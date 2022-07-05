
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
            // 구글 플레이 서비스에 로그인되어 있지 않으면 로그인 패널을 띄움
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

    // 구글 플레이 서비스에 로그인이 되어있는지 확인하는 함수

    public static bool IsAuthenticated()
    {
        return Social.localUser.authenticated;
    }

}
