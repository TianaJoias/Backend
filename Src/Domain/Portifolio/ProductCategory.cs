using System;

namespace Domain
{
    public class ProductCategory
    {
        public Guid ProductId { get; set; }
        public Guid TagId { get; set; }
        public Product Product { get; set; }
        public Tag Tag { get; set; }
        public ProductCategory()
        {

        }
        public ProductCategory(Product product, Tag tag)
        {
            Product = product;
            ProductId = product.Id;
            Tag = tag;
            TagId = Tag.Id;
        }
    }
}
