using Entities.Framework;
using Entities.MasterSignal;
using WebFramework.Api;

namespace API.Models
{
    public class UserDataDto : BaseDto<UserDataDto, UserData, int>
    {
        public string MobileNumber { get; set; }
        public string ApiKey { get; set; }
        public string SecretKey { get; set; }
        //public string PhrasePassword { get; set; }
    }
}
