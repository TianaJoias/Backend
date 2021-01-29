using System;
using System.Collections.Generic;
using Domain.Catalog;

namespace Domain.Portifolio
{
    public class Product : BaseEntity
    {
        protected Product() { }
        public Product(string sku, string description)
        {
            SKU = sku;
            Description = description;
            CreatedAt = Clock.Now;
            AddEvent(new NewProductEvent(this));
        }

        public string Description { get; set; }
        public DateTime CreatedAt { get; }
        public IList<Tag> Tags { get; set; } = new List<Tag>();
        public string SKU { get; set; }

        public void AddTag(Tag tag)
        {
            Tags.Add(tag);
        }

        public void ClearTags()
        {
            Tags.Clear();
        }
    }

    public class NewProductEvent : BaseEvent
    {
        public NewProductEvent(Product product)
        {
            SKU = product.SKU;
            ProductId = product.Id;
        }

        public string SKU { get; }
        public Guid ProductId { get; }
    }
}
