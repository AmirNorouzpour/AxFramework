using System;
using Entities.Framework;
using FluentValidation;

namespace Entities.MasterSignal
{
    public class UserData : BaseEntity
    {
        public string MobileNumber { get; set; }
        public string ApiKey { get; set; }
        public string SecretKey { get; set; }
        public string PhrasePassword { get; set; }
        public DateTime ExpireDate { get; set; }
        public decimal InitBalance { get; set; }
        public decimal Balance { get; set; }
    }

    public class UserDataValidator : AbstractValidator<UserData>
    {
    }

}
