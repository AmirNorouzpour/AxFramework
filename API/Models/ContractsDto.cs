using Entities.Contracts;
using WebFramework.Api;

namespace API.Models
{
    public class ContractsDto : BaseDto<ContractsDto, Contract, int>
    {
        public string F1 { get; set; }
        public string F2 { get; set; }
        public string F3 { get; set; }
        public string F4 { get; set; }
        public string F5 { get; set; }

    }
}
