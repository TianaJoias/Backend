using System.Collections.Generic;
using Application.Specification;

namespace Application.Common
{
    public class QueryPaged {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public Dictionary<string, SortDirection> Sort { get; set; }
    }
    public class QueryPaged<TResult> : QueryPaged, IQuery<PagedList<TResult>>
    {

    }
}
