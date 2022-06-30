using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GoogleNickNamePanel : MonoBehaviour
{
    [Header("NickName")]
    [SerializeField] private InputField displayNickName;
    [SerializeField] private Button displayNicknameCheckButton;

    [SerializeField] private CheckPanel checkPanel;
    bool displayNicknameCheck;

    bool isDisplayNickNameDuplication;

    bool isFinishDisplaynicknameCheckFunction = false;

    public void OnDisplayNickNameConfirmClick()
    {
        StartCoroutine("DisplayNicknameCheckPanelShow");
        IsDisplayNickNameDuplication(DisplayDuplicationCheck);
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
        if (displayNicknameCheck)
            SetUserDataInDataBase();
        yield return null;
    }
    public void CheckPanelCloseBtn()
    {
        checkPanel.gameObject.SetActive(false);
    }

    private void SetUserDataInDataBase()
    {
        DBData dBData = new DBData(DatabaseManager.instance.dbDataGoogle.Email.ToString(), displayNickName.text, "0", "false");
        DatabaseManager.instance.SetUserDataInDataBase(dBData);

    }
}
