using System;
using System.Collections.Generic;
using Domain.Portifolio;
using Domain.Stock;

namespace Domain.Catalog
{
    public class Catalog : BaseEntity
    {
        private List<CatalogItem> _items;

        protected Catalog()
        {
            _items = new List<CatalogItem>();
        }

        public Catalog(Agent channel) : this()
        {
            Channel = channel;
            Opened = Clock.Now;
        }

        public Agent Channel { get; private set; }
        public IReadOnlyCollection<CatalogItem> Items => _items.AsReadOnly();
        public DateTime Opened { get; private set; }
        public DateTime? Closed { get; private set; }
        public decimal TotalSold { get; set; }
        public void Add(Product produt, Lot lot, decimal quantity)
        {
            if (!Closed.HasValue) return;
            _items.Add(new CatalogItem(produt, lot, quantity));
        }

        public void Remaining(Guid productId, decimal quantity)
        {
            if (!Closed.HasValue) return;
            var item = _items.Find(it => it.ProdutoId == productId);    
            item.Remaining(quantity);
            TotalSold += item.TotalSold;
        }

        public void Close()
        {
            Closed = Clock.Now;
        }
    }
}
