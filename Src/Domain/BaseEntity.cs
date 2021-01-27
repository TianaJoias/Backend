using System;
using System.Collections.Generic;
using MediatR;

namespace Domain
{
    public abstract class BaseEvent: INotification
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public bool Handled { get; set; }
        public int Retries { get; set; }
    }

    public abstract class BaseEntity : IEntity
    {
        private readonly List<BaseEvent> _events = new();
        public IReadOnlyCollection<BaseEvent> Events => _events.AsReadOnly();
        public Guid Id { get; set; }  = Guid.NewGuid();
        public void AddEvent(BaseEvent @event)
        {
            _events.Add(@event);
        }

        public void ClearEvents()
        {
            _events.Clear();
        }
    }
}
