using Application.Common;

namespace Application.Products.Commands
{
    public class ProductQuery : QueryPaged<ProductQueryResult>
    {
        public string SearchTerm { get; set; }
    }
}
