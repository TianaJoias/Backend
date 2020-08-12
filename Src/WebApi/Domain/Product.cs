using System;
using System.Collections.Generic;

namespace WebApi.Domain
{
    public class Product : BaseEntity
    {
        public string BarCode { get; set; }
        public string Description { get; set; }
        public IList<int> Typologies { get; set; }
        public IList<int> Colors { get; set; }
        public IList<int> Categories { get; set; }
        public IList<int> Thematics { get; set; }
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
    }
}
