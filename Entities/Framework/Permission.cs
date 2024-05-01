using System.ComponentModel.DataAnnotations.Schema;
using FluentValidation;

namespace Entities.Framework
{
    public class Permission : BaseEntity
    {
        public bool Access { get; set; }
        public int MenuId { get; set; }
        [ForeignKey("MenuId")]
        public virtual Menu Menu { get; set; }
        public int? UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User User{ get; set; }
        public int? GroupId { get; set; }
        [ForeignKey("GroupId")]
        public virtual AxGroup Group { get; set; }
    }

    public class PermissionValidator : AbstractValidator<Permission> { }
}
