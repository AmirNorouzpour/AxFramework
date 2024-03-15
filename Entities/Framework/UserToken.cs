using System;
using Dapper.Contrib.Extensions;
using FluentValidation;

namespace Entities.Framework
{
    [Table("UserTokens")]
    public class UserToken : BaseEntity
    {
        public string Token { get; set; }
        public bool Active { get; set; }
        public string DeviceName { get; set; }
        public string Ip { get; set; }
        public string Browser { get; set; }
        public string UserAgent { get; set; }
        public string ClientId { get; set; }
        public int UserId { get; set; }
        public DateTime ExpireDateTime { get; set; }
    }
    public class UserTokenValidator : AbstractValidator<UserToken> { }
}
