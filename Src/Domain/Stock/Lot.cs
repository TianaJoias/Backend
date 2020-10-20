using System;
using System.Collections.Generic;

namespace Domain.Stock
{
    public class Lot : BaseEntity
    {
        public virtual Guid ProductId { get; set; }
        public decimal CostPrice { get; set; }
        public decimal SalePrice { get; set; }
        public decimal Quantity { get; set; }
        public decimal? Weight { get; set; }
        public virtual IList<Supplier> Suppliers { get; set; }
        public DateTime Date { get; set; }
        public string Number { get; set; }
        public string EAN { get; set; }
    }
}
