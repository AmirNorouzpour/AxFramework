using System;
using Entities.Framework;
using WebFramework.Api;

namespace API.Models
{
    public class UserSelectDto : BaseDto<UserSelectDto, User, int>
    {
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? ExpireDateTime { get; set; }
        public bool IsActive { get; set; }
        public DateTimeOffset? LastLoginDate { get; set; }

    }
}
