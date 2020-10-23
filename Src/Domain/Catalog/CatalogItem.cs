using System;
using System.Collections.Generic;
using Domain.Portifolio;
using Domain.Stock;

namespace Domain.Catalog
{
    public class CatalogItem : BaseEntity
    {
        protected CatalogItem() { }
        public CatalogItem(Product product, Lot lot, decimal quantity)
        {
            EAN = lot.EAN;
            Enabled = true;
            LongDescription = product.Description;
            Price = lot.SalePrice;
            ShortDescription = product.Description;
            ProdutoId = lot.ProductId;
            LotId = lot.Id;
            CurrentQuantity = quantity;
            InitialQuantity = quantity;
            SKU = product.SKU;
        }
        public Guid LotId { get; private set; }
        public Guid ProdutoId { get; private set; }
        public decimal InitialQuantity { get; private set; }
        public decimal CurrentQuantity { get; private set; }
        public decimal Price { get; private set; }
        public string SKU { get; private set; }
        public string EAN { get; private set; }
        public string LongDescription { get; private set; }
        public decimal TotalSold { get; private set; }

        public string ShortDescription { get; private set; }
        public IList<string> Thumbnail { get; private set; }
        public bool Enabled { get; private set; }

        public void Remaining(decimal quantity)
        {
            CurrentQuantity -= quantity;
            TotalSold = (InitialQuantity - CurrentQuantity) * Price;
            AddEvent(new ProductSoldEvent(quantity, LotId, ProdutoId, Price));
        }
    }

    public class ProductSoldEvent : BaseEvent
    {
        public ProductSoldEvent(decimal quantity, Guid lotId, Guid produtoId, decimal price)
        {
            Quantity = quantity;
            LotId = lotId;
            ProdutoId = produtoId;
            Price = price;
        }

        public decimal Quantity { get; }
        public Guid LotId { get; }
        public Guid ProdutoId { get; }
        public decimal Price { get; }
    }
}
