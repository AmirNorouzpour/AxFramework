using Dapper.Contrib.Extensions;
using FluentValidation;

namespace Entities.Framework
{
    [Table("LoginLogs")]
    public class LoginLog : BaseEntity
    {
        public string UserName { get; set; }
        public string InvalidPassword { get; set; }
        public string Browser { get; set; }
        public string BrowserVersion { get; set; }
        public string Os { get; set; }
        public int? UserId { get; set; }
        public string AppVersion { get; set; }
        public string Ip { get; set; }
        public string MachineName { get; set; }
        public bool ValidSignIn { get; set; }
    }

    public class LoginLogValidator : AbstractValidator<LoginLog> { }
}
