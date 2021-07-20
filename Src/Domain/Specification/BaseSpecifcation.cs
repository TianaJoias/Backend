using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Domain.Specification
{
    public class BaseSpecifcation<T> : ISpecificationPaged<T>
    {
        public BaseSpecifcation()
        {
        }
        public BaseSpecifcation(Expression<Func<T, bool>> criteria)
        {
            Criteria = criteria;
        }
        public Expression<Func<T, bool>> Criteria { get; }
        public List<Expression<Func<T, object>>> Includes { get; } = new List<Expression<Func<T, object>>>();
        public
        List<(Expression<Func<T, object>>, SortDirection)> OrderBy
        { get; private set; } = new List<(Expression<Func<T, object>>, SortDirection)>();

        public int PageNumber { get; private set; }
        public int PageSize { get; private set; }
        protected void Page(int number, int size)
        {
            PageNumber = number;
            PageSize = size;
        }
        protected void AddInclude(Expression<Func<T, object>> includeExpression)
        {
            Includes.Add(includeExpression);
        }
        protected void AddOrderBy(Expression<Func<T, object>> orderByExpression)
        {
            OrderBy.Add((orderByExpression, SortDirection.Asc));
        }
        protected void AddOrderByDescending(Expression<Func<T, object>> orderByDescExpression)
        {
            OrderBy.Add((orderByDescExpression, SortDirection.Desc));
        }
    }
}
