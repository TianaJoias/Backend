using System;
using System.Collections.Generic;

namespace WebApi.Domain
{
    public class Product : BaseEntity
    {
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
