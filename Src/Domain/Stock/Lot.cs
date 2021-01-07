using System;
using System.Collections.Generic;
using System.Linq;
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
        public string EAN { get; set; }

        protected Lot()
        {

        }
        public Lot(Guid productId, decimal costPrice, decimal salePrice, decimal quantity, string number, IList<Supplier> supplier)
        {
            ProductId = productId;
            CostPrice = costPrice;
            SalePrice = salePrice;
            Quantity = quantity;
            CurrentyQuantity = quantity;
            Number = number;
            CreatedAt = Clock.Now;
            Suppliers = supplier;
            AddEvent(new NewLotEvent(this));
        }
        public void Withdraw(decimal quantity)
        {
            CurrentyQuantity -= quantity;
        }
    }


    public class NewLotEvent : BaseEvent
    {
        public NewLotEvent(Lot lot)
        {
            CreatedAt = lot.CreatedAt;
            ProductId = lot.ProductId;
            CostPrice = lot.CostPrice;
            SalePrice = lot.SalePrice;
            EAN = lot.EAN;
            Quantity = lot.Quantity;
            SuppliersId = lot.Suppliers.Select(it => it.Id);
            Number = lot.Number;
        }

        public DateTime CreatedAt { get; }
        public Guid ProductId { get; }
        public decimal CostPrice { get; }
        public decimal SalePrice { get; }
        public string EAN { get; }
        public decimal Quantity { get; }
        public IEnumerable<Guid> SuppliersId { get; }
        public string Number { get; }
    }
}
