using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Entities.Framework
{
    public class Strategy : BaseEntity
    {
        public string Name { get; set; }
        [Precision(18, 8)]
        public decimal OrderSize { get; set; }
        [Precision(18, 8)]
        public decimal InitialCapital { get; set; }
        public string BaseCurrency { get; set; }
        public int Pyramiding { get; set; }
        public OrderSizeType OrderSizeType { get; set; }
        [Precision(18, 8)]
        public decimal Commission { get; set; }
    }

    public enum OrderSizeType
    {
        Contract,
        Composite,
        MonetaryUnit

    }

    public class UserStrategy : BaseEntity
    {
        public string Name { get; set; }
        public string Data { get; set; }
        public int Version { get; set; }
        [ForeignKey("UserId")]
        public int UserId { get; set; }
        public User User { get; set; }
        public string Unique { get; set; }
    }

    public class UserStrategyValidator : AbstractValidator<UserStrategy>
    {
        public UserStrategyValidator()
        {
            RuleFor(x => x.Name).NotNull();
            RuleFor(x => x.Data).NotNull();
        }
    }
}
