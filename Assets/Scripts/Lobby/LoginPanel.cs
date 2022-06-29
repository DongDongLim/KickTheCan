using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
#if UNITY_ANDROID
using GooglePlayGames;
using GooglePlayGames.BasicApi;
#endif

public class LoginPanel : MonoBehaviour
{
    public GameObject googleLoginBtn;

    //https://geukggom.tistory.com/155

    void Start()
    {
#if UNITY_ANDROID
        // 구글 플레이 서비스에 로그인되어 있지 않으면 로그인 패널을 띄움
        if (!IsAuthenticated())
            googleLoginBtn.SetActive(true);
#endif
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


#if UNITY_ANDROID
    
    // 구글 플레이 서비스에 로그인이 되어있는지 확인하는 함수
    
    public static bool IsAuthenticated()
    {
        return Social.localUser.authenticated;
    }

    // 로그인 버튼을 눌렀을 때
    public void Start_Auth()
    {
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
            //.EnableSavedGames() // 게임 진행 상황을 저장할 수 있게 함
            //.WithInvitationDelegate(< callback method >) // 게임이 꺼져 있을 때, 게임 초대 수신을 위한 콜백
            //.WithMatchDelegate(< callback method >) // 게임이 꺼져 있을 때, 턴 기반 매치 알림을 수신을 위한 콜백
            //.RequestEmail() // 플레이어의 이메일 주소를 사용할 수 있도록 요청 + 동의를 요청하는 메시지를 띄움
            //.RequestServerAuthCode( false ) // 연결된 백엔드 서버 애플리케이션에 전달되고 OAuth 토큰으로 교환될 수 있도록 서버 인증 코드를 생성하도록 요청
            //.RequestIdToken() // ID 토큰 생성을 요청(Firebase에서 플레이어를 식별하는 데 사용)
            .Build();

        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.DebugLogEnabled = true; // recommended for debugging
        PlayGamesPlatform.Activate(); // Activate the Google Play Games platform

        Social.localUser.Authenticate((bool success) =>
        {
            // handle success or failure
            if (success)
                OnLoginButtonClicked(); //로그인 성공 - 로그인 패널 꺼짐
            else 
                Debug.Log("Google Login fail"); //로그인 실패
        });
    }


#endif
}
