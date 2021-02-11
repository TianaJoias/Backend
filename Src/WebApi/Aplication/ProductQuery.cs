namespace WebApi.Aplication
{
    public class ProductQuery : FilterPagedQuery<PagedData<ProductQueryResult>>
    {
        public string SearchTerm { get; set; }
    }
}
