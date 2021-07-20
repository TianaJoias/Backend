using System;
using Application.Common;

namespace Application.Stock.Queries.ProductSuppliers
{
    public class ProductSupplierQuery: QueryPaged<ProductSupplierResult>
    {
        public Guid ProductId { get; set; }
    }
}
