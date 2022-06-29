﻿using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Firebase.Auth;

public class SignUpPanel : MonoBehaviour
{
    [Header("email")]
    [SerializeField] private InputField emailInputField;
    bool emailCheck;

    [Header("password")]
    [SerializeField] private InputField passwordInputField;
    [SerializeField] private Text passwordMessage;
    bool passwordCheck;

    [Header("passwordConfirm")]
    [SerializeField] private InputField passwordConfirmInputField;
    [SerializeField] private Text passwordConfirmMessage;
    bool passwordConfirmCheck;


    [Header("NickName")]
    [SerializeField] private InputField displayNickName;
    [SerializeField] private Button displayNicknameCheckButton;
    bool displayNicknameCheck;



    [SerializeField] private Button SignUpButton;
    [SerializeField] private CheckPanel CheckBox;

    [SerializeField] private SignUpComplete signUpCompletePanel;
    [SerializeField] private CheckPanel checkPanel;


    bool isEmailDuplication;
    bool isDisplayNickNameDuplication;


    bool isFinishEmailCheckFunction;
    bool isFinishDisplaynicknameCheckFunction;
    public void OnEnable()
    {
        emailInputField.text = "";
        passwordInputField.text = "";
        passwordConfirmInputField.text = "";
        displayNickName.text = "";

        passwordMessage.text = "6~20 비밀번호 입력";
        passwordConfirmMessage.text = "비밀번호 확인";

        emailCheck = false;
        passwordCheck = false;
        passwordConfirmCheck = false;
        displayNicknameCheck = false;
        isEmailDuplication = false;
        SignUpButton.interactable = false;
        signUpCompletePanel.gameObject.SetActive(false);
        checkPanel.gameObject.SetActive(false);
        isFinishEmailCheckFunction = false;
        isFinishDisplaynicknameCheckFunction = false;
    }


    
    public void OnEmailConfirmClick()
    {
        StartCoroutine("EmailCheckPanelShow");
        IsEmailDuplication(EmailCheckClick);
    }


    public void IsEmailDuplication(UnityAction OnCheck)
    {

        DatabaseManager.instance.reference = FirebaseDatabase.DefaultInstance.GetReference("UserInfo");

        DatabaseManager.instance.reference.GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                isEmailDuplication = false;
                foreach (DataSnapshot data in snapshot.Children)
                {

                    IDictionary userInfo = (IDictionary)data.Value;
                    if ((string)userInfo["Email"] == emailInputField.text)
                    {
                        isEmailDuplication = true;
                        break;
                    }
                }
                isFinishEmailCheckFunction = true;
                OnCheck.Invoke();
            }
        });
    }


    IEnumerator EmailCheckPanelShow()
    {

        while(!isFinishEmailCheckFunction)
        {

            yield return new WaitForSeconds(0.05f);
        }
        checkPanel.CanUse(emailInputField.text, emailCheck);
        checkPanel.gameObject.SetActive(true);
        isFinishEmailCheckFunction = false;

        yield return null;
    }

    public void EmailCheckClick()
    {
        if (isEmailDuplication)
        {
            Debug.Log("중복된 이메일 입니다");
            emailCheck = false;
        }
        else if (IsValidEmail(emailInputField.text))
        {
            Debug.Log("정상적인 이메일입니다.");
            emailCheck = true;
        }
        else
        {
            Debug.Log("잘못된 이메일 형식 입니다");
            emailCheck = false;
        }

        SignUpInteractable();
    }




    public bool IsValidEmail(string email)
    {
        bool valid = Regex.IsMatch(email, @"[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?\.)+[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?");
        return valid;
    }




    public void OnDisplayNickNameConfirmClick()
    {
        StartCoroutine("DisplayNicknameCheckPanelShow");
        IsDisplayNickNameDuplication(DisplayDuplicationCheck);
    }

    IEnumerator DisplayNicknameCheckPanelShow()
    {

        while (!isFinishDisplaynicknameCheckFunction)
        {
            yield return new WaitForSeconds(0.05f);
        }
        checkPanel.CanUse(displayNickName.text, displayNicknameCheck);
        checkPanel.gameObject.SetActive(true);
        isFinishDisplaynicknameCheckFunction = false;

        yield return null;
    }


    public void DisplayDuplicationCheck()
    {
        if (isDisplayNickNameDuplication)
        {
            Debug.Log("중복된 닉네임");
            displayNicknameCheck = false;

        }
        else
        {
            Debug.Log("사용 가능한 닉네임");
            displayNicknameCheck = true;

        }
        SignUpInteractable();
    }




    public void IsDisplayNickNameDuplication(UnityAction OnCheck)
    {
        DatabaseManager.instance.reference = FirebaseDatabase.DefaultInstance.GetReference("UserInfo");

        DatabaseManager.instance.reference.GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                isDisplayNickNameDuplication = false;
                foreach (DataSnapshot data in snapshot.Children)
                {
                    IDictionary userInfo = (IDictionary)data.Value;
                    if ((string)userInfo["DisplayNickname"] == displayNickName.text)
                    {
                        isDisplayNickNameDuplication = true;
                        
                        break;
                    }
                }
                isFinishDisplaynicknameCheckFunction = true;
                OnCheck.Invoke();
            }
        });
    }






    public void PasswordCheck()
    {
        if (passwordInputField.text.Length >= 6 && passwordInputField.text.Length <= 20)
        {
            passwordMessage.text = "사용 가능";
            passwordCheck = true;
        }
        else
        {
            passwordMessage.text = "잘못된 비밀번호";
            passwordCheck = false;
        }
        SignUpInteractable();
    }



    public void PasswordConfirmCheck()
    {
        if (passwordConfirmInputField.text == passwordInputField.text)
        {
            passwordConfirmMessage.text = "일치 합니다.";
            passwordConfirmCheck = true;
        }
        else
        {
            passwordConfirmMessage.text = "비밀번호가 일치하지 않습니다.";
            passwordConfirmCheck = false;
        }
        SignUpInteractable();
    }
    

    public void EmailInputFieldCheck()
    {
        emailCheck = false;
        SignUpInteractable();
    }
    

    public void DisplayNicknameInputFieldCheck()
    {
        displayNicknameCheck = false;
        SignUpInteractable();
    }




    private void SetUserDataInDataBase()
    {
        DBData newUser = new DBData(emailInputField.text, displayNickName.text, "0","false");
        AuthManager.instance.register(emailInputField.text, passwordInputField.text, newUser);
       
    }





    private void SignUpInteractable()
    {
        if (emailCheck &&
        passwordCheck &&
        passwordConfirmCheck &&
        displayNicknameCheck
        )
            SignUpButton.interactable = true;
        else
        {
            SignUpButton.interactable = false;
        }
    }




    public void SignUpButtonClicked()
    {
        SetUserDataInDataBase();
        ShowCompletePanel();

    }


    private void ShowCompletePanel()
    {
        signUpCompletePanel.SetNameText(displayNickName.text);
        signUpCompletePanel.gameObject.SetActive(true);
    }
    

    public void ShowSignUpPanel()
    {
        this.gameObject.SetActive(true);
    }

    public void BackButtonClicked()
    {
        this.gameObject.SetActive(false);
    }

    public void CheckPanelCloseBtn()
    {
        checkPanel.gameObject.SetActive(false);
    }
}




