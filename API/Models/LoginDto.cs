﻿namespace API.Models
{
    public class LoginDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Captcha { get; set; }
    }

    public class UserConnectionDto
    {
        public string ConnectionId { get; set; }
    }
}
