using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Common;
using FluentValidation;


namespace Entities.Framework
{
    public class Menu : BaseEntity
    {
        public string Title { get; set; }
        public bool Active { get; set; }
        public bool ShowInMenu { get; set; }
        public int? ParentId { get; set; }
        public Menu Parent { get; set; }
        public string Key { get; set; }
        public string Icon { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Menu> Children { get; set; }
        public AxOp AxOp { get; set; }
        public int OrderId { get; set; }
        [NotMapped]
        public bool Checked { get; set; }
        [NotMapped]
        public bool IsLeaf { get; set; }
    }
    public class MenuValidator : AbstractValidator<Menu> { }
}
