using System.Collections.Generic;
using Domain;

namespace WebApi.Aplication
{
    public record PaginationQuery
    {
        public string SearchTerm { get; init; }
        public int Page { get; init; } = 1;
        public int PageSize { get; init; } = 5;
        public Dictionary<string, Sort> OrderBy { get; init; } = null;
    }
}
