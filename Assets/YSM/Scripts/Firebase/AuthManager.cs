using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Auth;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
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

    bool isFinishLogFunction;
    bool isWrong;
    [SerializeField] GameObject IDPasswordMismatchPanel;
    void Awake()
    {
        // 객체 초기화
        instance = this;
        auth = FirebaseAuth.DefaultInstance;
        isFinishLogFunction = false;
    }

    public void OnClickLogin()
    {
        StartCoroutine("ShowLogInMessage");
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
                    isWrong = false;
                }
                else
                {
                    // 비밀번호 6자리 미만 , 동일 이메일 있으면 로그인 실패 

                    //아이디 비번 확인 메세지 
                    isWrong = true;
                    Debug.Log("로그인에 실패하셨습니다.");
                }
                isFinishLogFunction = true;
            }
        );
    }
    //->로그인 버튼 누르면 - > // 비동기  로그인 실행 -> 로그인 된 나의 UID -> // 비동기 데이터 가져오기 -> //패널 넘어가는거  

    IEnumerator ShowLogInMessage()
    {

        while (!isFinishLogFunction)
            yield return new WaitForSeconds(0.05f);

        if(true == isWrong)
        {
            IDPasswordMismatchPanel.SetActive(true);
        }
        isFinishLogFunction = false;
        yield return null;
    }

    public void OnIDPasswordMismatchPanelCloseClicked()
    {
        IDPasswordMismatchPanel.SetActive(false);
    }


    public void register(string email, string password , DBData data)
    {
        
        // 제공되는 함수 : 이메일과 비밀번호로 회원가입 시켜 줌
        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(
            task => {
                if (!task.IsCanceled && !task.IsFaulted)
                {
                    Debug.Log(email+ "로 회원가입\n");
                    DatabaseManager.instance.SetUserDataInDataBase(data);
                    auth = FirebaseAuth.DefaultInstance; // TODO : 오류있을수도 있음 
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
        PlayGamesPlatform.InitializeInstance(new PlayGamesClientConfiguration.Builder()
            .RequestIdToken()
            .RequestEmail()
            .Build());
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();

        Social.localUser.Authenticate(success =>
        {
            // handle success or failure
            if (success)
            {
                StartCoroutine(TryFirebaseLogin()); //로그인 성공 - 로그인 패널 꺼짐 
            }
            else
                SceneManager.LoadScene(0);
        });
    }




    // 로그인 하잖아요 
    // UID 가지고 올 수 있으면 -> 이것부터 확인
    // Realtime database에 UID를 확인해서 
    // 있으면 -> 내 로컬 데이터를 Realtime database에 있는 정보로 세팅해주고 -> connect scene 이동
    // 없으면 -> 닉네임만 적는 패널을 만들어서 아이디 중복 검사만 하고 -> UI 를 aaaaaaaaaaaaaaaaa 세팅 해주고 이메일 스코어 뭐 기타등등 세팅해줘 -> connect scene이동 하고


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



