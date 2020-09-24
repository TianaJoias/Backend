using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Domain
{
    public interface IRepository<T> where T : IEntity
    {
        Task<T> GetById(Guid id);
        Task<List<T>> List();
        Task<List<T>> List(Expression<Func<T, bool>> filter);
        Task<bool> Exists(Expression<Func<T, bool>> filter);
        Task<T> Add(T entity);
        Task Update(T entity);
        Task Delete(T entity);
        Task<PagedResult<T>> GetPaged(Expression<Func<T, bool>> filter,
                                       int page, int pageSize, Func<IQueryable<T>, IOrderedQueryable<T>> ordering = null);
    }
    public class PagedResult<T>
    {
        public int CurrentPage { get; set; }
        public int PageCount { get; set; }
        public int PageSize { get; set; }
        public int RowCount { get; set; }
        public IList<T> Results { get; set; }
        public PagedResult()
        {
            Results = new List<T>();
        }
        public int FirstRowOnPage
        {

            get { return CurrentPage * PageSize + 1; }
        }

        public int LastRowOnPage
        {
            get { return Math.Min(CurrentPage * PageSize, RowCount); }
        }
    }

}
