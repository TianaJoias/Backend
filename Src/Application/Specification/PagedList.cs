using System;
using System.Collections.Generic;
using System.Linq;
using Mapster;

namespace Application.Specification
{
    public class PagedList<T>
    {
        public int CurrentPage { get; private set; }
        public int TotalPages { get; private set; }
        public int PageSize { get; private set; }
        public int TotalCount { get; private set; }
        public bool HasPrevious => CurrentPage > 1;
        public bool HasNext => CurrentPage < TotalPages;
        public IList<T> Items { get; private set; }
        public PagedList(IList<T> items, int totalCount, int pageNumber, int pageSize)
        {
            TotalCount = totalCount;
            PageSize = pageSize;
            CurrentPage = pageNumber;
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            Items = items;
        }

        public PagedList<TD> To<TD>() {

            var items = Items.Adapt<List<TD>>();
            var newInstance = new PagedList<TD>(items, TotalCount, CurrentPage, PageSize);
            return newInstance;
        }
        public static PagedList<T> ToPagedList(IQueryable<T> source, int pageNumber, int pageSize)
        {
            var count = source.Count();
            var items = source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            return new PagedList<T>(items, count, pageNumber, pageSize);
        }
    }
}
