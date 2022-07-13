using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Auth;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Firebase.Database;
#if UNITY_ANDROID
using GooglePlayGames;
using GooglePlayGames.BasicApi;
#endif

public class AuthManager : Singleton<AuthManager>
{

    [SerializeField] private InputField emailField;
    [SerializeField] private InputField passwordField;
    [SerializeField] private Button loginBtn;

    // 인증을 관리할 객체
    FirebaseAuth auth;
    FirebaseUser user;


    bool isFinishLogFunction;
    bool isFinishGoogleLogFunction;
    bool isWrong;

    [SerializeField] GameObject IDPasswordMismatchPanel;

    [SerializeField] GameObject[] canvas;
    protected override void OnAwake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        auth = FirebaseAuth.DefaultInstance;
        user = null;
        isFinishLogFunction = false;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "LobbyScene")
            foreach (GameObject obj in canvas)
                obj.SetActive(true);
        else
            foreach (GameObject obj in canvas)
                obj.SetActive(false);

    }

    public void OnClickLogin()
    {
        loginBtn.interactable = false;
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
                    user = auth.CurrentUser;
                    FriendManager.instance.SetEventListener();
                    isWrong = false;
                }
                else
                {
                    //아이디 비번 확인 메세지 
                    isWrong = true;
                    Debug.Log("로그인에 실패하셨습니다.");
                }
                isFinishLogFunction = true;
            }
        );
    }

    // GoogleBugFix / DH
    public void login(Credential credential, UnityAction OnCheck)
    {
        // 제공되는 함수 : 이메일과 비밀번호로 로그인 시켜 줌
        auth.SignInWithCredentialAsync(credential).ContinueWith(
            task => {
                if (task.IsCompleted && !task.IsFaulted && !task.IsCanceled)
                {
                    OnCheck.Invoke();
                    user = auth.CurrentUser;
                    isWrong = false;
                }
                else
                {
                    //아이디 비번 확인 메세지 
                    isWrong = true;
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
        loginBtn.interactable = true;

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

    public string GetCurrentUID()
    {
        return user.UserId;
    }

#if UNITY_ANDROID


    // 로그인 버튼을 눌렀을 때
    public void Start_Auth()
    {
        // 구글 플레이 서비스에 로그인되어 있으면 반환
        if (Social.localUser.authenticated)
            return;

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


    public bool isNickName = false;
    public IEnumerator TryFirebaseLogin()
    {
        isFinishGoogleLogFunction = true;
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
            isFinishGoogleLogFunction = false;
            // 파이어베이스 로그인 성공하고 uid로 닉네임 가져옴
            // null이면 닉네임 생성창
            // 있으면 DatabaseManager.instance.GetMyData?
        });
        while (isFinishGoogleLogFunction)
        {
            yield return null;
        }
        yield return StartCoroutine(DatabaseManager.instance.MyNickNameCheck());
        yield return null;
        if (isNickName)
            login(credential, DatabaseManager.instance.GetMyData);
        else
            LobbyManager.instance.SetActivePanel(LobbyManager.PANEL.NickName);
    }

    public FirebaseUser GetUser()
    {
        return auth.CurrentUser;
    }

#endif




    private void OnApplicationFocus(bool focus)
    {
        if (user == null)
            return;
        if (focus == true)
        {
            SetLogin(true);
        }
        else
        {
            SetLogin(false);
        }
    }

    public void OnApplicationQuit()
    {
        if (user == null)
            return;
        SetLogin(false);

    }

    public void SetLogin(bool isLogin) //true = login  // false = logout
    {

        DatabaseManager.instance.dbReference
            .Child("UserInfo")
            .Child(GetCurrentUID())
            .UpdateChildrenAsync(FuncTool.ConvertToIDictionary(DBData.KeyIsLogin, isLogin)); //로그인 로그아웃
    }


}



