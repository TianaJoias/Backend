using System;
using System.Collections.Generic;
using Domain.Portifolio;
using Domain.Stock;

namespace Domain.Catalog
{
    public class Catalog : BaseEntity
    {
        private readonly List<CatalogItem> _items;

        protected Catalog()
        {
            _items = new List<CatalogItem>();
        }

        public Catalog(Agent channel) : this()
        {
            Agent = channel;
            CreatedAt = Clock.Now;
            AddEvent(new CatalogOpenedEvent(this));
        }

        public Agent Agent { get; private set; }
        public IReadOnlyCollection<CatalogItem> Items => _items.AsReadOnly();
        public DateTime CreatedAt { get; private set; }
        public DateTime? ClosedAt { get; private set; }
        public decimal TotalSold { get; set; }
        public void Add(Product produt, Lot lot, decimal quantity)
        {
            if (ClosedAt.HasValue) return;
            _items.Add(new CatalogItem(produt, lot, quantity));
        }

        public void Remaining(Guid LotId, decimal quantity)
        {
            if (ClosedAt.HasValue) return;
            var item = _items.Find(it => it.LotId == LotId);    
            item.Remaining(quantity);
            TotalSold += item.TotalSold;
        }

        public void Close()
        {
            ClosedAt = Clock.Now;
            AddEvent(new CatalogClosedEvent(this));
        }
    }

    public class CatalogOpenedEvent : BaseEvent
    {
        public CatalogOpenedEvent(Catalog catalog)
        {
            CreatedAt = catalog.CreatedAt;
            AgentId = catalog.Agent.Id;
        }

        public DateTime CreatedAt { get; }
        public Guid AgentId { get; }
    }

    public class CatalogClosedEvent : BaseEvent
    {
        public CatalogClosedEvent(Catalog catalog)
        {
            ClosedAt = catalog.ClosedAt.Value;
            OpenedAt = catalog.CreatedAt;
            TotalSold = catalog.TotalSold;
            AgentId = catalog.Agent.Id;
        }

        public DateTime ClosedAt { get; }
        public DateTime OpenedAt { get; }
        public decimal TotalSold { get; }
        public Guid AgentId { get; }
    }
}
