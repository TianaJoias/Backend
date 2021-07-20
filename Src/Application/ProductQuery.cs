using Application.Common;

namespace Application
{
    public class ProductQuery : QueryPaged<ProductQueryResult>
    {
        public string SearchTerm { get; set; }
    }
}
