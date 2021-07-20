using Domain;
using Domain.Specification;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Infra.EF.Repositories
{
    public class SpecificationEvaluator<TEntity> where TEntity : class
    {
        public static IQueryable<TEntity> GetQuery(IQueryable<TEntity> inputQuery, ISpecification<TEntity> spec)
        {
            var query = inputQuery;
            if (spec.Criteria != null)
            {
                query = query.Where(spec.Criteria);
            }
            if (spec.OrderBy != null && spec.OrderBy.Any())
            {
                var (expression, direction) = spec.OrderBy.First();
                IOrderedQueryable<TEntity> query2;
                if (direction == SortDirection.Asc)
                    query2 = query.OrderBy(expression);
                else
                    query2 = query.OrderByDescending(expression);
                spec.OrderBy.RemoveAt(0);
                query = spec.OrderBy.Aggregate(query2, (current, include) => include.direction == SortDirection.Asc ? current.ThenBy(include.expression) : current.ThenByDescending(include.expression));
            }

            query = spec.Includes.Aggregate(query, (current, include) => current.Include(include));
            return query;
        }
    }
}
