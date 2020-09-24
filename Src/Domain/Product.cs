using System;
using System.Collections.Generic;

namespace Domain
{
    /// <summary>
    ///  ef migrations add --project .\Src\Infra\Infra.csproj --startup-project .\Src\WebApi\WebApi.csproj -o EF\Migrations\SqlLite --context TianaJoiasContextDB InitialBackend
    /// </summary>
    public class Product : BaseEntity
    {
        public string BarCode { get; set; }
        public string Description { get; set; }
        public IList<ProductCategory> Categories { get; set; } = new List<ProductCategory>();

        public void AddCategory(Tag tag)
        {
            Categories.Add(new ProductCategory(this, tag));
        }

        public void RemoveAllCategories()
        {
            Categories.Clear();
        }
    }

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

    public class Tag : BaseEntity
    {
        public string Name { get; set; }
    }

    public class Batch : BaseEntity
    {
        public virtual Product Product { get; set; }
        public decimal CostValue { get; set; }
        public decimal SaleValue { get; set; }
        public decimal Quantity { get; set; }
        public decimal? Weight { get; set; }
        public virtual IList<Supplier> Suppliers { get; set; }
        public DateTime Date { get; set; }
        public string Number { get; set; }
    }

    public class Supplier : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public virtual IList<Batch> Batchs { get; set; }
    }
}
