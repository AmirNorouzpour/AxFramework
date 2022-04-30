using System.ComponentModel.DataAnnotations.Schema;
using FluentValidation;

namespace Entities.Framework
{
    public class UserChart : BaseEntity
    {
        public int UserId { get; set; }
        public int OrderIndex { get; set; }
        public int Width { get; set; }
        public bool Active { get; set; }
        public int Height { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }
    }
    public class UserChartValidator : AbstractValidator<UserChart> { }
}
