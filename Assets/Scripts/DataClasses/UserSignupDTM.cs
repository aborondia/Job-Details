public class UserSignupDTM
{
    public string username { get; set; }
    public string email { get; set; }
    public string password { get; set; }

    public UserSignupDTM(string user, string email, string password)
    {
        this.username = user;
        this.email = email;
        this.password = password;
    }
}
