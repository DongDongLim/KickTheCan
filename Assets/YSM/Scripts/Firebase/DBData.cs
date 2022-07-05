
public class DBData
{

    static public string KeyDisplayNickname = "DisplayNickname";
    static public string KeyScore = "Score";
    static public string KeyEmail = "Email";
    static public string KeyIsLogin = "IsLogin";


    public string DisplayNickname;
    public int Score;
    public string Email;
    public bool IsLogin;
    public DBData(string Email , string DisplayNickname, int Score, bool IsLogin = false)
    {
        this.Email = Email;
        this.DisplayNickname = DisplayNickname;
        this.Score = Score;
        this.IsLogin = IsLogin;
    }

}
