using Entities.Framework;
using FluentValidation;

namespace Entities.Contracts
{
    public class Contract : BaseEntity
    {
        public string F1 { get; set; }
        public string F2 { get; set; }
        public string F3 { get; set; }
        public string F4 { get; set; }
        public string F5 { get; set; }
    }



    public class ContractValidator : AbstractValidator<Contract> { }
}
