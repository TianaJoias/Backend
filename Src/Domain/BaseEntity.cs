using System;
using System.Collections.Generic;
using MediatR;

namespace Domain
{
    public abstract class BaseEvent: INotification
    {
        public DateTime CreateAt { get; private set; } = Clock.Now;
        public bool Published { get; private set; } = false;

        public void Publish()
        {
            Published = true;
        }
    }

    public abstract class BaseEntity : IEntity
    {
        private readonly List<BaseEvent> _events = new();
        public IReadOnlyCollection<BaseEvent> Events => _events.AsReadOnly();
        public Guid Id { get; protected set; } = Guid.NewGuid();
        protected void AddEvent(BaseEvent @event)
        {
            _events.Add(@event);
        }
    }
}
