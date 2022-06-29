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
        // ���� �÷��� ���񽺿� �α��εǾ� ���� ������ �α��� �г��� ���
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
    
    // ���� �÷��� ���񽺿� �α����� �Ǿ��ִ��� Ȯ���ϴ� �Լ�
    
    public static bool IsAuthenticated()
    {
        return Social.localUser.authenticated;
    }

    // �α��� ��ư�� ������ ��
    public void Start_Auth()
    {
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
            //.EnableSavedGames() // ���� ���� ��Ȳ�� ������ �� �ְ� ��
            //.WithInvitationDelegate(< callback method >) // ������ ���� ���� ��, ���� �ʴ� ������ ���� �ݹ�
            //.WithMatchDelegate(< callback method >) // ������ ���� ���� ��, �� ��� ��ġ �˸��� ������ ���� �ݹ�
            //.RequestEmail() // �÷��̾��� �̸��� �ּҸ� ����� �� �ֵ��� ��û + ���Ǹ� ��û�ϴ� �޽����� ���
            //.RequestServerAuthCode( false ) // ����� �鿣�� ���� ���ø����̼ǿ� ���޵ǰ� OAuth ��ū���� ��ȯ�� �� �ֵ��� ���� ���� �ڵ带 �����ϵ��� ��û
            //.RequestIdToken() // ID ��ū ������ ��û(Firebase���� �÷��̾ �ĺ��ϴ� �� ���)
            .Build();

        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.DebugLogEnabled = true; // recommended for debugging
        PlayGamesPlatform.Activate(); // Activate the Google Play Games platform

        Social.localUser.Authenticate((bool success) =>
        {
            // handle success or failure
            if (success)
                OnLoginButtonClicked(); //�α��� ���� - �α��� �г� ����
            else 
                Debug.Log("Google Login fail"); //�α��� ����
        });
    }


#endif
}
