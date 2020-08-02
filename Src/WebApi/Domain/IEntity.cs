using System;

namespace WebApi.Domain
{
    public interface IEntity
    {
        Guid Id { get; }
    }
}
