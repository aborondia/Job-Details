public class UserSignInDTM
{
    public string userName { get; set; }
    public string password { get; set; }

    public UserSignInDTM(string user, string passWord)
    {
        this.userName = user;
        this.password = passWord;
    }
}
