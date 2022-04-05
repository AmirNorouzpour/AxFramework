using System;
using System.ComponentModel.DataAnnotations.Schema;
using Entities.Framework;
using FluentValidation;

namespace Entities.Tracking
{
    public class ProductInstanceHistory : BaseEntity
    {
        public int ProductInstanceId { get; set; }
        [ForeignKey("ProductInstanceId")]
        public ProductInstance ProductInstance { get; set; }
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }
        public int PersonnelId { get; set; }
        [ForeignKey("PersonnelId")]
        public Personnel Personnel { get; set; }
        public int? MachineId { get; set; }
        [ForeignKey("MachineId")]
        public Machine Machine { get; set; }
        public int Day { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public int ShiftId { get; set; }
        [ForeignKey("ShiftId")]
        public Shift Shift { get; set; }
        public DateTime EnterTime{ get; set; }
        public DateTime? ExitTime { get; set; }
    }


    public class ProductInstanceHistoryValidator : AbstractValidator<ProductInstanceHistory>
    {
        public ProductInstanceHistoryValidator()
        {
            RuleFor(x => x.Id).NotNull();
        }
    }
}
