using System;
using System.Linq.Expressions;

namespace Domain.Specification
{
    public class SpecifcationBuilder<T>
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
        private SpecifcationBuilder(Expression<Func<T, bool>> criteria)
        {
            _spec = new _Spec(criteria);
        }

        public static SpecifcationBuilder<T> Where(Expression<Func<T, bool>> expression) => new SpecifcationBuilder<T>(expression);
        public static SpecifcationBuilder<T> All() => new SpecifcationBuilder<T>(it => true);
        public SpecifcationBuilder<T> WithPage(int number, int size)
        {
            _spec.WithPage(number, size);
            return this;
        }
        public SpecifcationBuilder<T> Include(Expression<Func<T, object>> includeExpression)
        {
            _spec.Include(includeExpression);
            return this;
        }
        public SpecifcationBuilder<T> WithOrderBy(Expression<Func<T, object>> orderByExpression)
        {
            _spec.WithOrderBy(orderByExpression);
            return this;
        }
        public SpecifcationBuilder<T> WithOrderByDescending(Expression<Func<T, object>> orderByDescExpression)
        {
            _spec.WithOrderByDescending(orderByDescExpression);
            return this;
        }
        public BaseSpecifcation<T> Build() => _spec;
    }
}
