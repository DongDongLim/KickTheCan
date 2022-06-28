
public class myData
{
    public string DisplayNickname;
    public string Score;
    public string Email;
    public string IsLoggingIn;
    public myData(string Email , string DisplayNickname, string Score, string IsLoggingIn = "true")
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
