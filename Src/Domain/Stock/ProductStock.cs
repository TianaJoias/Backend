using System.Collections.Generic;
using System;

namespace Domain.Stock
{
    public class ProductStock : BaseEntity
    {

        protected ProductStock() { }
        public ProductStock(Guid productId)
        {
            ProductId = productId;
        }

        public Guid ProductId { get; set; }
        public decimal Quantity { get; set; }

        public void Withdraw(decimal quantity)
        {
            Quantity -= quantity;
        }

        public void Deposit(decimal quantity)
        {
            Quantity += quantity;
        }
    }
}
