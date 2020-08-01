using System;

namespace WebApi.Infra
{
    public interface IEntity
    {
        Guid Id { get; }
    }

    public abstract class BaseEntity : IEntity
    {
        public Guid Id { get; set; }
    }
}
