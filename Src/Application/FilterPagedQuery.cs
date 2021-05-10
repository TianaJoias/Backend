using System.Collections.Generic;
using Domain;

namespace WebApi.Aplication
{
    public class FilterPagedQuery<T> : FilterPaged, IQuery<T>
    {
        public FilterPagedQuery() { }
        public FilterPagedQuery((string key, Sort value) defaultOrder) : base(defaultOrder) { }
    }

    public class FilterPaged
    {
        public int Page { get; init; } = 1;
        public int PageSize { get; init; } = 10;
        public Dictionary<string, Sort> OrderBy { get; init; }
        public FilterPaged() { }
        public FilterPaged((string key, Sort value) defaultOrder)
        {
            OrderBy = new Dictionary<string, Sort>
            {
                { defaultOrder.key, defaultOrder.value }
            };
        }
    }

    public record PagedData<T>(int CurrentPage, int TotalPages, int TotalRows, IList<T> Data);
}
