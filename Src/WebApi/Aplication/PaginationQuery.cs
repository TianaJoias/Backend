using System.Collections.Generic;
using Domain;

namespace WebApi.Aplication
{
    public class PaginationQuery
    {
        public string SearchTerm { get; set; }
        public int Page { get; set; } = 1; public int PageSize { get; set; } = 5; public Dictionary<string, Sort> OrderBy { get; set; } = null;
    }
}
