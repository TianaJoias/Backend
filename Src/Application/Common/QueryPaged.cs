using System.Collections.Generic;
using Domain;
using Domain.Specification;

namespace Application.Common
{
    public class QueryPaged<TResult> : IQuery<PagedList<TResult>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }

        public Dictionary<string, SortDirection> Sort { get; set; }
    }
}
