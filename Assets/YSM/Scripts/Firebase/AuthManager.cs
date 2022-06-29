using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Auth;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.Events;
#if UNITY_ANDROID
using GooglePlayGames;
using GooglePlayGames.BasicApi;
#endif

public class AuthManager : MonoBehaviour
{


    [SerializeField] private InputField emailField;
    [SerializeField] private InputField passwordField;

    // 인증을 관리할 객체
    FirebaseAuth auth;

    static public AuthManager instance { get; private set; }



    void Awake()
    {
        // 객체 초기화
        instance = this;
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
    }

    public void OnClickLogin()
    {
        login(emailField.text, passwordField.text, DatabaseManager.instance.GetMyData);
    }


    public void login(string email , string password, UnityAction OnCheck)
    {
        // 제공되는 함수 : 이메일과 비밀번호로 로그인 시켜 줌
        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(
            task => {
                if (task.IsCompleted && !task.IsFaulted && !task.IsCanceled)
                {
                    Debug.Log(email + " 로 로그인 하셨습니다.");
                    OnCheck.Invoke();
                }
                else
                {
                    Debug.Log("로그인에 실패하셨습니다.");
                }

            }
        );
    }
    //->로그인 버튼 누르면 - > // 비동기  로그인 실행 -> 로그인 된 나의 UID -> // 비동기 데이터 가져오기 -> //패널 넘어가는거  

    public void register(string email, string password , DBData data)
    {
        
        // 제공되는 함수 : 이메일과 비밀번호로 회원가입 시켜 줌
        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(
            task => {
                if (!task.IsCanceled && !task.IsFaulted)
                {
                    Debug.Log(email+ "로 회원가입\n");
                    DatabaseManager.instance.SetUserDataInDataBase(data);
                    auth = FirebaseAuth.DefaultInstance; 
                }
                else
                    Debug.Log("회원가입 실패\n");
            }
        );
    }


    public string GetAuthUID()
    {
        return auth.CurrentUser.UserId;
    }

#if UNITY_ANDROID

    // 구글 플레이 서비스에 로그인이 되어있는지 확인하는 함수


    // 로그인 버튼을 눌렀을 때
    public void Start_Auth()
    {
        //PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
        //    //.EnableSavedGames() // 게임 진행 상황을 저장할 수 있게 함
        //    //.WithInvitationDelegate(< callback method >) // 게임이 꺼져 있을 때, 게임 초대 수신을 위한 콜백
        //    //.WithMatchDelegate(< callback method >) // 게임이 꺼져 있을 때, 턴 기반 매치 알림을 수신을 위한 콜백
        //    //.RequestEmail() // 플레이어의 이메일 주소를 사용할 수 있도록 요청 + 동의를 요청하는 메시지를 띄움
        //    //.RequestServerAuthCode( false ) // 연결된 백엔드 서버 애플리케이션에 전달되고 OAuth 토큰으로 교환될 수 있도록 서버 인증 코드를 생성하도록 요청
        //    .RequestIdToken() // ID 토큰 생성을 요청(Firebase에서 플레이어를 식별하는 데 사용)
        //    .Build();

        //PlayGamesPlatform.InitializeInstance(config);
        //PlayGamesPlatform.DebugLogEnabled = true; // recommended for debugging
        //PlayGamesPlatform.Activate(); // Activate the Google Play Games platform

        PlayGamesPlatform.InitializeInstance(new PlayGamesClientConfiguration.Builder()
            .RequestIdToken()
            .RequestEmail()
            .Build());
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();

        Social.localUser.Authenticate((bool success) =>
        {
            // handle success or failure
            if (success)
            {
                StartCoroutine(TryFirebaseLogin()); //로그인 성공 - 로그인 패널 꺼짐
            }
            else
                Application.Quit();
        });
    }

    IEnumerator TryFirebaseLogin()
    {
        while (string.IsNullOrEmpty(((PlayGamesLocalUser)Social.localUser).GetIdToken()))
            yield return null;
        string idToken = ((PlayGamesLocalUser)Social.localUser).GetIdToken();


        Credential credential = GoogleAuthProvider.GetCredential(idToken, null);
        auth.SignInWithCredentialAsync(credential).ContinueWith(task => {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInWithCredentialAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInWithCredentialAsync encountered an error: " + task.Exception);
                return;
            }

            Debug.Log("Success!");
        });
    }


#endif

}



