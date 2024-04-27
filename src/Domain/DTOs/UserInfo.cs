namespace Application.DTOs;

public class UserInfo
{
    public class Login
    {
        public string Username { get; set; }
        public string Password { get; set; }   
    }

    public class Register
    {
        public string Username { get; set; }
        public string Password { get; set; }   
        public string Email { get; set; }
    }
}