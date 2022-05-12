using System;
using System.ComponentModel.DataAnnotations.Schema;
using Entities.Framework;
using FluentValidation;

namespace Entities.Tracking
{
    public class Damaged : BaseEntity
    {
        public int ProductInstanceId { get; set; }
        [ForeignKey("ProductInstanceId")]
        public ProductInstance ProductInstance { get; set; }
        public int PersonnelId { get; set; }
        [ForeignKey("PersonnelId")]
        public Personnel Personnel { get; set; }
        public DateTime DateTime { get; set; }
        public string DamageCode { get; set; }
    }

    public class DamagedValidator : AbstractValidator<Damaged>
    {
        public DamagedValidator()
        {
            RuleFor(x => x.Id).NotNull();
        }
    }
}
