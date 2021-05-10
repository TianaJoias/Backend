using System;

namespace WebApi.Aplication.Stock.Queries.ProductSuppliers
{
    public class ProductSupplierResult
    {
        public Guid SupplierId { get; set; }
        public Guid ProductId { get; set; }
        public string Code { get; set; }
        public string SupplierName { get; set; }
    }
}
