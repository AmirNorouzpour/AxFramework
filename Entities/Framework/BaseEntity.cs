using System;

namespace Entities.Framework
{
    public interface IEntity
    {
    }

    public abstract class BaseEntity<TKey> : IEntity
    {
        public TKey Id { get; set; }
        public DateTime InsertDateTime { get; set; }
        public DateTime? ModifiedDateTime { get; set; }
        public int CreatorUserId { get; set; }
        public int? ModifierUserId { get; set; }
    }

    public abstract class BaseEntity : BaseEntity<int>
    {
    }
}
