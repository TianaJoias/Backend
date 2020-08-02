using System;

namespace WebApi.Domain
{
    public abstract class BaseEntity : IEntity
    {
        public Guid Id { get; set; }
    }
}
