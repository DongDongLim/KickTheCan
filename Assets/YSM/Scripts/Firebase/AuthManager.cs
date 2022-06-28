using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Auth;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.Events;

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

    public void register(string email, string password , myData data)
    {
        // 제공되는 함수 : 이메일과 비밀번호로 회원가입 시켜 줌
        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(
            task => {
                if (!task.IsCanceled && !task.IsFaulted)
                {
                    Debug.Log(email+ "로 회원가입\n");
                    DatabaseManager.instance.SetUserDataInDataBase(data);
                    auth = Firebase.Auth.FirebaseAuth.DefaultInstance; 
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
   
}



                                                                                                                                                 