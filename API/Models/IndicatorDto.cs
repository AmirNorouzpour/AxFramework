using System;
using Entities.Framework;
using WebFramework.Api;

namespace API.Models
{
    public class IndicatorDto : BaseDto<IndicatorDto, Indicator, int>
    {
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? ExpireDateTime { get; set; }
        public bool IsActive { get; set; }
        public DateTimeOffset? LastLoginDate { get; set; }

    }
}
