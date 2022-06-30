
public class DBData
{

    static public string KeyDisplayNickname = "DisplayNickname";
    static public string KeyScore = "Score";
    static public string KeyEmail = "Email";
    static public string KeyIsLoggingIn = "IsLoggingIn";


    public string DisplayNickname;
    public string Score;
    public string Email;
    public string IsLoggingIn;
    public DBData(string Email , string DisplayNickname, string Score, string IsLoggingIn = "true")
    {
        this.Email = Email;
        this.DisplayNickname = DisplayNickname;
        this.Score = Score;
        this.IsLoggingIn = IsLoggingIn;
    }


    public void SetLogout()
    {
        IsLoggingIn = "false";
    }
}
