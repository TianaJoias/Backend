using System;

namespace Domain
{
    public abstract class BaseEntity : IEntity
    {
        public Guid Id { get; set; }
    }
}
