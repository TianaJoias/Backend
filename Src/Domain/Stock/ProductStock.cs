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
        public decimal ReservedQuantity { get; set; }
        public decimal TotalWithdrawal { get; set; }

        public void Reserve(decimal quantity){
            Withdraw(quantity);
            ReservedQuantity += quantity;
        }

        public void Return(decimal quantity){
            Deposit(quantity);
            ReservedQuantity -= quantity;
        }
        public void Withdraw(decimal quantity)
        {
            TotalWithdrawal += quantity;
            Quantity -= quantity;
        }
        public void ConfirmSale(decimal quantity){
            ReservedQuantity -= quantity;
        }
        public void Deposit(decimal quantity)
        {
            Quantity += quantity;
        }
    }
}
