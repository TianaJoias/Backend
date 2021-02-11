using System;
using System.Collections.Generic;

namespace WebApi.Aplication.Stock.Queries.ProductSuppliers
{
    public class ProductSupplierQuery: FilterPagedQuery<PagedData<ProductSupplierResult>>
    {
        public Guid ProductId { get; set; }
    }
}
