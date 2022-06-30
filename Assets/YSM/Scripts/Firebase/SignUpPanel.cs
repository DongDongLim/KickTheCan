using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using Firebase.Auth;

public class SignUpPanel : MonoBehaviour
{
    [Header("email")]
    [SerializeField] private InputField emailInputField;
    [SerializeField] private TextMeshProUGUI emailMessage;
    [SerializeField] private Image emailCheckImage01;
    [SerializeField] private Image emailCheckImage02;
    bool emailCheck;

    int emailChecker;

    [Header("password")]
    [SerializeField] private InputField passwordInputField;
    [SerializeField] private TextMeshProUGUI passwordMessage;
    bool passwordCheck;

    [Header("passwordConfirm")]
    [SerializeField] private InputField passwordConfirmInputField;
    [SerializeField] private TextMeshProUGUI passwordConfirmMessage;
    bool passwordConfirmCheck;


    [Header("NickName")]
    [SerializeField] private InputField displayNickName;
    [SerializeField] private TextMeshProUGUI nicknameMessage;
    [SerializeField] private Image nickCheckImage01;
    [SerializeField] private Image nickCheckImage02;
    [SerializeField] private Button displayNicknameCheckButton;
    bool displayNicknameCheck;

    int nicknameChecker;


    [SerializeField] private Button SignUpButton;
    [SerializeField] private CheckPanel CheckBox;

    [SerializeField] private SignUpComplete signUpCompletePanel;
    [SerializeField] private CheckPanel checkPanel;

    int signUpButtonChecker;


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

        emailMessage.text = "";
        passwordMessage.text = "";
        passwordConfirmMessage.text = "";
        nicknameMessage.text = "";

        emailCheckImage01.color = Color.black;
        emailCheckImage02.color = Color.black;
        nickCheckImage01.color = Color.black;
        nickCheckImage02.color = Color.black;
        
        emailCheck = false;
        passwordCheck = false;
        passwordConfirmCheck = false;
        displayNicknameCheck = false;
        isEmailDuplication = false;
        SignUpButton.interactable = false;
        signUpCompletePanel.gameObject.SetActive(false);
        
        signUpButtonChecker = 0;

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
            emailChecker = 0;
            emailCheck = false;
        }
        else if (IsValidEmail(emailInputField.text))
        {
            Debug.Log("사용 가능한 이메일입니다");
            emailChecker = 1;
            emailCheck = true;
        }
        else
        {
            Debug.Log("잘못된 이메일 형식 입니다");
            emailChecker = 2;
            emailCheck = false;
        }

        SignUpInteractable();
    }

#region EmailChecker
    public void EmailChecker()
    {
        StartCoroutine(EmailCheckerCoroutine());
    }

    IEnumerator EmailCheckerCoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        switch (emailChecker)
        {
            case 0:
                Debug.Log("New0");
                emailMessage.text = "중복된 이메일 입니다";
                emailMessage.color = Color.red;
                emailCheckImage01.color = Color.red;
                emailCheckImage02.color = Color.red;
                break;
            case 1:
                emailMessage.text = "사용 가능한 이메일입니다";
                emailMessage.color = Color.green;
                emailCheckImage01.color = Color.green;
                emailCheckImage02.color = Color.green;
                break;
            case 2:
                emailMessage.text = "잘못된 이메일 형식 입니다";
                emailMessage.color = Color.red;
                emailCheckImage01.color = Color.red;
                emailCheckImage02.color = Color.red;
                break;
        }
    }
#endregion



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
            nicknameChecker = 0;
            displayNicknameCheck = false;

        }
        else
        {
            nicknameChecker = 1;
            displayNicknameCheck = true;

        }
        SignUpInteractable();
    }

#region NickNameChecker
    public void NickNameChecker()
    {
        StartCoroutine(NickNameCheckerCoroutine());
    }

    IEnumerator NickNameCheckerCoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        switch (nicknameChecker)
        {
            case 0:
                nicknameMessage.text = "중복된 닉네임 입니다";
                nicknameMessage.color = Color.red;
                nickCheckImage01.color = Color.red;
                nickCheckImage02.color = Color.red;
                break;
            case 1:
                nicknameMessage.text = "사용 가능한 닉네임 입니다";
                nicknameMessage.color = Color.green;
                nickCheckImage01.color = Color.green;
                nickCheckImage02.color = Color.green;
                break;
        }
    }
#endregion


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
            passwordMessage.text = "사용 가능한 비밀번호 입니다";
            passwordMessage.color = Color.green;
            passwordCheck = true;
        }
        else
        {
            passwordMessage.text = "6 ~ 20 자리로 설정해 주세요";
            passwordMessage.color = Color.red;
            passwordCheck = false;
        }
        SignUpInteractable();
    }



    public void PasswordConfirmCheck()
    {
        if (passwordConfirmInputField.text == passwordInputField.text)
        {
            passwordConfirmMessage.text = "비밀번호가 일치 합니다";
            passwordConfirmMessage.color = Color.green;
            passwordConfirmCheck = true;
        }
        else
        {
            passwordConfirmMessage.text = "비밀번호가 일치하지 않습니다";
            passwordConfirmMessage.color = Color.red;
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
            signUpButtonChecker = 1;
        else
        {
            signUpButtonChecker = 0;
        }
        StartCoroutine(SignUpInteractableCoroutine());
    }

    IEnumerator SignUpInteractableCoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        Debug.Log("사인업버튼");
        switch(signUpButtonChecker)
        {
            case 0:
                SignUpButton.interactable = false;
                break;
            case 1:
                SignUpButton.interactable = true;
                break;
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




