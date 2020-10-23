using System;
using System.Collections.Generic;
using Domain.Catalog;

namespace Domain.Stock
{
    public class Lot : BaseEntity
    {
        public virtual Guid ProductId { get; private set; }
        public decimal CostPrice { get; private set; }
        public decimal SalePrice { get; private set; }
        public decimal Quantity { get; private set; }
        public decimal CurrentyQuantity { get; private set; }
        public decimal? Weight { get; private set; }
        public virtual IList<Supplier> Suppliers { get; private set; } = new List<Supplier>();
        public DateTime CreatedAt { get; private set; }
        public string Number { get; private set; }
        public string EAN { get; private set; }

        protected Lot()
        {

        }
        public Lot(Guid productId, decimal costPrice, decimal salePrice, decimal quantity, string number, string EAN, IList<Supplier> supplier)
        {
            ProductId = productId;
            CostPrice = costPrice;
            SalePrice = salePrice;
            Quantity = quantity;
            CurrentyQuantity = quantity;
            Number = number;
            this.EAN = EAN;
            CreatedAt = Clock.Now;
            Suppliers = supplier;
        }

        public void Withdraw(decimal quantity)
        {
            CurrentyQuantity -= quantity;
        }
    }
}
