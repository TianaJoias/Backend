using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using GraphQL;
using GraphQL.Authorization;
using GraphQL.Builders;
using GraphQL.Types;
using GraphQL.Utilities;
using Microsoft.Extensions.DependencyInjection;
using WebApi.Aplication;

namespace WebApi.Queries
{

    public class ProductType : ObjectGraphType<Product>
    {
        public ProductType()
        {
            Name = "Product";
            Field(x => x.Id, type: typeof(GuidGraphType)).Description("Product Id");
            Field(x => x.BarCode).Description("Bar Code of Product");
            Field(x => x.Description).Description("Description of product");
            Field<ListGraphType<GuidGraphType>>("Categories", description: "Description of product", resolve: context =>
            {
                return context.Source.Categories.Select(it => it.TagId);
            });
        }
    }
    public class PagedResultType<T, TType> : ObjectGraphType<PagedResult<T>>
        where TType : IGraphType
    {
        public PagedResultType()
        {
            Name = "Page";
            Field(x => x.CurrentPage).Description("Bar Code of Product");
            Field(x => x.PageSize).Description("Description of product");
            Field(x => x.PageCount).Description("Description of product");
            Field<ListGraphType<TType>>("Results");
        }
    }

    public class ProductSortType : InputObjectGraphType
    {
        public ProductSortType()
        {
            Name = "ProductSort";
            Field<EnumerationGraphType<Sort>>("Id");
            Field<EnumerationGraphType<Sort>>("BarCode");
            Field<EnumerationGraphType<Sort>>("Description");
        }
    }


    public class ProductQuery : ObjectGraphType<object>
    {
        public string ToTitleCase(string str)
        {
            return str.First().ToString().ToUpper() + str.Substring(1);
        }
        public ProductQuery()
        {
            Field<PagedResultType<Product, ProductType>>("Product",
                 arguments: new QueryArguments(new QueryArgument[]
                 {
                    new QueryArgument<GuidGraphType>{Name="id"},
                    new QueryArgument<StringGraphType>{Name="description"},
                    new QueryArgument<NonNullGraphType<IntGraphType>>{Name="page", DefaultValue = 0},
                    new QueryArgument<NonNullGraphType<IntGraphType>>{Name="pageSize", DefaultValue = 10},
                    new QueryArgument<ProductSortType>{Name="sortBy", DefaultValue = new Dictionary<string, object> { { "Description", Sort.Asc } }  }
                 }), resolve: context => context.RunScopedAsync(Run));
        }

        private async Task<object> Run(IResolveFieldContext<object> context, IServiceProvider serviceProvider)
        {
            var repository = serviceProvider.GetService<IProductRepository>();
            var page = context.GetArgument<int>("page");
            var pageSize = context.GetArgument<int>("pageSize");
            var reservationId = context.GetArgument<Guid?>("id");
            if (reservationId.HasValue)
                return await repository.GetById(reservationId.Value);

            var sortBy = context.GetArgument<Dictionary<string, object>>("sortBy").ToDictionary(entry => entry.Key,
                                                 entry => (Sort)entry.Value);
            var description = context.GetArgument<string>("description");
            if (!string.IsNullOrWhiteSpace(description))
                return await repository.GetPaged(it => it.Description.Contains(description), page, pageSize, OrderByHelper.BuildOrderBy<Product>(sortBy, p => p.Id));
            return await repository.GetPaged(it => true, page, pageSize, OrderByHelper.BuildOrderBy<Product>(sortBy, p => p.Id));
        }
    }

    public class TianaJoiasSchema : Schema
    {
        public TianaJoiasSchema(IServiceProvider serviceProvider)
        : base(serviceProvider)
        {
            Query = serviceProvider.GetService<ProductQuery>();
            Query.AuthorizeWith("AdminPolicy");
        }
    }

    public static class ContextExtensions
    {
        public static async Task<TReturn> RunScopedAsync<TSource, TReturn>(
            this IResolveFieldContext<TSource> context,
            Func<IResolveFieldContext<TSource>, IServiceProvider, Task<TReturn>> func)
        {
            using var scope = context.RequestServices.CreateScope();
            return await func(context, scope.ServiceProvider);
        }
    }

    public static class FieldBuilderExtensions
    {
        public static FieldBuilder<TSource, TReturn> ResolveScopedAsync<TSource, TReturn>(
            this FieldBuilder<TSource, TReturn> builder,
            Func<IResolveFieldContext<TSource>, IServiceProvider, Task<TReturn>> func)
        {
            return builder.ResolveAsync(context => context.RunScopedAsync(func));
        }
    }
}
