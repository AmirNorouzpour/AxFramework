using System;
using System.Collections.Generic;
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
        public string FireBaseToken { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string UniqueKey { get; set; }
        public GenderType GenderType { get; set; }
        public DateTime? BirthDate { get; set; }
        public DateTime? ExpireDateTime { get; set; }
        public bool OutOfOrgAccess { get; set; }
        public DateTimeOffset? LastLoginDate { get; set; }
        public ICollection<Address> Addresses { get; set; }
        public UserSetting UserSettings { get; set; }
        [NotMapped]
        public string FullName => FirstName + " " + LastName;
    }

    public enum GenderType
    {
        Female = 0,
        Male = 1,
        None = 2
    }

    public class UserValidator : AbstractValidator<User>
    {
        public UserValidator() {
            RuleFor(x => x.Id).NotNull();
        }
    }

}
