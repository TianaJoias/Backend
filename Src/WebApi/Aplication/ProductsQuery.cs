using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Domain;
using Mapster;
using MediatR;

namespace WebApi.Aplication
{
    public class ProductsQuery : IQuery<ProductQueryInDTO, ProductsQueryOutDTO>
    {
        private readonly IProductRepository _productRepository;

        public ProductsQuery(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<ProductsQueryOutDTO> Handle(ProductQueryInDTO request, CancellationToken cancellationToken)
        {
            Expression<Func<Product, bool>> query = it => it.Description.Contains(request.SearchTerm);
            if (string.IsNullOrWhiteSpace(request.SearchTerm))
                query = it => true;
            var result = await _productRepository.GetPaged(query, request.Page, request.PageSize,
                request.BuildOrderBy<Product>(p => p.Id));
            return result.Adapt<ProductsQueryOutDTO>();
        }
    }
    public class ProductsQueryById : IQuery<ProductQueryByIdRequest, ProductDTO>
    {
        private readonly IProductRepository _productRepository;

        public ProductsQueryById(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<ProductDTO> Handle(ProductQueryByIdRequest request, CancellationToken cancellationToken)
        {
            var result = await _productRepository.GetById(request.Id);
            return result.Adapt<ProductDTO>();
        }
    }

    public static class OrderByHelper
    {
        public static Func<IQueryable<T>, IOrderedQueryable<T>> BuildOrderBy<T>(this QueryInBase request, Expression<Func<T, object>> defaultOrdeBy)
        {
            return (IQueryable<T> query) =>
            {
                return OrderBy(query, GetPropertyInfo(request.OrderBy, defaultOrdeBy), request.Order == Sort.Desc);
            };
        }

        public static Func<IQueryable<T>, IOrderedQueryable<T>> BuildOrderBy<T>(IDictionary<string, Sort> request, Expression<Func<T, object>> defaultOrdeBy)
        {
            return (IQueryable<T> query) =>
            {
                foreach (var item in request)
                {
                    query = teste(query, item, defaultOrdeBy);
                }
                return query as IOrderedQueryable<T>;
            };
        }
        public static IOrderedQueryable<T> teste<T>(IQueryable<T> query, KeyValuePair<string, Sort> item, Expression<Func<T, object>> defaultOrdeBy)
        {
            return OrderBy(query, GetPropertyInfo(item.Key, defaultOrdeBy), item.Value == Sort.Desc);
        }

        private static PropertyInfo GetPropertyInfo<TSource>(string propertyName, Expression<Func<TSource, object>> defaultProperty)
        {
            if (String.IsNullOrWhiteSpace(propertyName))
                return GetPropertyInfo(defaultProperty); var entityType = typeof(TSource);

            var propertyInfo = entityType.GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (propertyInfo is null)
                return GetPropertyInfo(defaultProperty);
            if (propertyInfo.DeclaringType != entityType)
            {
                propertyInfo = propertyInfo.DeclaringType.GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            }

            // If we try to order by a property that does not exist in the object return the list
            return propertyInfo;
        }
        private static PropertyInfo GetPropertyInfo<TPropertySource>
    (Expression<Func<TPropertySource, object>> expression)
        {
            var lambda = expression as LambdaExpression;
            MemberExpression memberExpression;
            if (lambda.Body is UnaryExpression)
            {
                var unaryExpression = lambda.Body as UnaryExpression;
                memberExpression = unaryExpression.Operand as MemberExpression;
            }
            else
            {
                memberExpression = lambda.Body as MemberExpression;
            }

            if (memberExpression != null)
            {
                var propertyInfo = memberExpression.Member as PropertyInfo;

                return propertyInfo;
            }

            return null;
        }
        private static MethodInfo _OrderBy = typeof(Queryable).GetMethods()
                 .Where(m => m.Name == "OrderBy" && m.IsGenericMethodDefinition)
                 .Where(m => m.GetParameters().ToList().Count == 2) // ensure selecting the right overload
                 .Single();
        private static MethodInfo _OrderByDescending = typeof(Queryable).GetMethods()
             .Where(m => m.Name == "OrderByDescending" && m.IsGenericMethodDefinition)
             .Where(m => m.GetParameters().ToList().Count == 2) // ensure selecting the right overload
             .Single();
        private static MethodInfo _ThenBy = typeof(Queryable).GetMethods()
             .Where(m => m.Name == "ThenBy" && m.IsGenericMethodDefinition)
             .Where(m => m.GetParameters().ToList().Count == 2) // ensure selecting the right overload
             .Single();
        private static MethodInfo _ThenByDescending = typeof(Queryable).GetMethods()
             .Where(m => m.Name == "ThenByDescending" && m.IsGenericMethodDefinition)
             .Where(m => m.GetParameters().ToList().Count == 2) // ensure selecting the right overload
             .Single();
        private static IOrderedQueryable<TSource> OrderBy<TSource>(IQueryable<TSource> query, PropertyInfo propertyInfo, bool isDesc)
        {
            if (propertyInfo is null)
                return (IOrderedQueryable<TSource>)query;

            var entityType = typeof(TSource);

            var arg = Expression.Parameter(entityType, "x");
            var property = Expression.MakeMemberAccess(arg, propertyInfo);
            var selector = Expression.Lambda(property, new ParameterExpression[] { arg });
            try
            {
                //The linq's OrderBy<TSource, TKey> has two generic types, which provided here
                MethodInfo genericMethod = (isDesc ? _ThenByDescending : _ThenBy).MakeGenericMethod(entityType, propertyInfo.PropertyType);

                /* Call query.OrderBy(selector), with query and selector: x=> x.PropName
                  Note that we pass the selector as Expression to the method and we don't compile it.
                  By doing so EF can extract "order by" columns and generate SQL for it. */
                return (IOrderedQueryable<TSource>)genericMethod.Invoke(genericMethod, new object[] { query, selector });
            }
            catch
            {
                //The linq's OrderBy<TSource, TKey> has two generic types, which provided here
                MethodInfo genericMethod = (isDesc ? _OrderByDescending : _OrderBy).MakeGenericMethod(entityType, propertyInfo.PropertyType);

                /* Call query.OrderBy(selector), with query and selector: x=> x.PropName
                  Note that we pass the selector as Expression to the method and we don't compile it.
                  By doing so EF can extract "order by" columns and generate SQL for it. */
                return (IOrderedQueryable<TSource>)genericMethod.Invoke(genericMethod, new object[] { query, selector });
            }
        }
    }


    public enum Sort { Asc, Desc }
    public abstract class QueryInBase
    {
        public int Page { get; set; } = 0;
        public int PageSize { get; set; } = 5;
        public string SearchTerm { get; set; }
        public string OrderBy { get; set; }
        public Sort Order { get; set; }
    }


    public class ProductQueryInDTO : QueryInBase, IRequest<ProductsQueryOutDTO>
    {
    }
    public class ProductQueryByIdRequest : IRequest<ProductDTO>
    {
        public Guid Id { get; set; }
    }

    public class ProductsQueryOutDTO : PagedResult<ProductDTO>
    {
    }

    public class ProductDTO
    {
        public Guid? Id { get; set; }
        public string BarCode { get; set; }
        public string Description { get; set; }
        public IList<Guid> Categories { get; set; }
    }

    public interface IQuery<TInput, TOutput> : IRequestHandler<TInput, TOutput> where TInput : IRequest<TOutput>
    {
    }
}
