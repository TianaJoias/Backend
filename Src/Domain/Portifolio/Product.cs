using System;
using System.Collections.Generic;

namespace Domain.Portifolio
{
    public class Product : BaseEntity
    {
        protected Product() { }
        public Product(string sku, string description)
        {
            SKU = sku;
            Description = description;
            AddEvent(new NewProductEvent(this));
        }

        public string Description { get; set; }
        public IList<Tag> Categories { get; set; } = new List<Tag>();
        public string SKU { get; set; }

        public void AddCategory(Tag tag)
        {
            Categories.Add(tag);
        }

        public void RemoveAllCategories()
        {
            Categories.Clear();
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
