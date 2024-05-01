using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FluentValidation;

namespace Entities.Framework
{
    public class User : BaseEntity
    {
        [Required]
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool IsActive { get; set; } = true;
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UniqueKey { get; set; }
        public string FireBaseToken { get; set; }
        public string Email { get; set; }
        public LoginType LoginType { get; set; }
        public DateTime? ExpireDateTime { get; set; }
        public DateTime LastLoginDate { get; set; }
        public UserSetting UserSettings { get; set; }
        [NotMapped]
        public string FullName => FirstName + " " + LastName;
    }

    public enum LoginType
    {
        Email,
        Google
    }


    public class UserValidator : AbstractValidator<User>
    {
        public UserValidator()
        {
            RuleFor(x => x.Id).NotNull();
        }
    }

}
