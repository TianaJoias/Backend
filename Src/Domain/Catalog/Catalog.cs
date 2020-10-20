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

        public Catalog(Channel channel) : this()
        {
            Channel = channel;
        }

        public Channel Channel { get; private set; }
        public IReadOnlyCollection<CatalogItem> Items => _items.AsReadOnly();
        public DateTime Opened { get; private set; }
        public DateTime? Closed { get; private set; }

        public void Add(Product produt, Lot lot, decimal quantity)
        {
            _items.Add(new CatalogItem(produt, lot, quantity));
        }
    }
}
