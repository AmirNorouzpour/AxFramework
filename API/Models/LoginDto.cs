namespace API.Models
{
    public class LoginDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Key { get; set; }
    }

    public class UserConnectionDto
    {
        public string ConnectionId { get; set; }
    }


    public class FBTokenDto
    {
        public string Token { get; set; }
    }
}
