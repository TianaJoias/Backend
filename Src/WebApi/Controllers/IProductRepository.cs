using System;
using System.Collections.Generic;
using WebApi.Infra;

namespace WebApi.Controllers
{
    public interface IProductRepository : IRepository<Product>
    {
    }

    public class Product : BaseEntity
    {
        public Guid Id { get; set; }
        public string BarCode { get; set; }
        public string Description { get; set; }
        public decimal CostValue { get; set; }
        public decimal SalePrice { get; set; }
        public decimal Quantity { get; set; }
        public decimal? Weight { get; set; }
        public Guid Supplier { get; set; }
        public IList<int> Typologies { get; set; }
        public IList<int> Colors { get; set; }
        public IList<int> Categories { get; set; }
        public IList<int> Thematics { get; set; }
    }
}