using System;
using System.Linq.Expressions;

namespace Application.Specification
{
    public class SpecificationBuilder<T>
    {
        private class _Spec : BaseSpecifcation<T>
        {
            public _Spec(Expression<Func<T, bool>> criteria) : base(criteria)
            {
            }
            public void WithPage(int number, int size)
            {
                Page(number, size);
            }
            public void Include(Expression<Func<T, object>> includeExpression)
            {
                AddInclude(includeExpression);
            }
            public void WithOrderBy(Expression<Func<T, object>> orderByExpression)
            {
                AddOrderBy(orderByExpression);
            }
            public void WithOrderByDescending(Expression<Func<T, object>> orderByDescExpression)
            {
                AddOrderByDescending(orderByDescExpression);
            }
        }
        private readonly _Spec _spec;
        private SpecificationBuilder(Expression<Func<T, bool>> criteria)
        {
            _spec = new _Spec(criteria);
        }

        public static SpecificationBuilder<T> Where(Expression<Func<T, bool>> expression) => new SpecificationBuilder<T>(expression);
        public static SpecificationBuilder<T> All() => new(it => true);
        public SpecificationBuilder<T> WithPage(int number, int size)
        {
            _spec.WithPage(number, size);
            return this;
        }
        public SpecificationBuilder<T> Include(Expression<Func<T, object>> includeExpression)
        {
            _spec.Include(includeExpression);
            return this;
        }
        public SpecificationBuilder<T> WithOrderBy(Expression<Func<T, object>> orderByExpression)
        {
            _spec.WithOrderBy(orderByExpression);
            return this;
        }
        public SpecificationBuilder<T> WithOrderByDescending(Expression<Func<T, object>> orderByDescExpression)
        {
            _spec.WithOrderByDescending(orderByDescExpression);
            return this;
        }
        public BaseSpecifcation<T> Build() => _spec;
    }
}
