using System;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Types;
using GraphQL.Utilities;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using WebApi.Aplication;
using WebApi.Domain;

namespace WebApi.Queries
{

    public class ProductType : ObjectGraphType<Product>
    {
        public ProductType()
        {
            Name = "Product";
            Field(x => x.Id, type: typeof(GuidGraphType)).Description("Product Id");
            Field(x => x.BarCode).Description("Bar Code of Product");
            Field(x => x.Categories).Description("Nome do usuário");
            Field(x => x.Colors).Description("Data criação usuário");
            Field(x => x.Description).Description("Data alteração usuário");
            Field(x => x.Thematics).Description("Data alteração usuário");
            Field(x => x.Typologies).Description("Data alteração usuário");
        }
    }
    public class ProductQuery : ObjectGraphType<object>
    {
        public ProductQuery(IProductRepository repositorio)
        {

            Field<ListGraphType<ProductType>>("Product",
                 arguments: new QueryArguments(new QueryArgument[]
                 {
                    new QueryArgument<IdGraphType>{Name="id"},
                    new QueryArgument<StringGraphType>{Name="description"},
                    new QueryArgument<NonNullGraphType<IntGraphType>>{Name="page", DefaultValue = 0},
                    new QueryArgument<NonNullGraphType<IntGraphType>>{Name="pageSize", DefaultValue = 10},
                 }),
                 resolve: context =>
                 {
                     var page = context.GetArgument<int>("page");
                     var pageSize = context.GetArgument<int>("pageSize");
                     var reservationId = context.GetArgument<Guid?>("id");
                     if (reservationId.HasValue)
                         return repositorio.GetById(reservationId.Value);
                     var description = context.GetArgument<string>("description");
                     if (!string.IsNullOrWhiteSpace(description))
                         return repositorio.List(it => it.Description.Contains(description));
                    //return repositorio.GetPaged(it => it.Description.Contains(description), page, pageSize,);
                     return null;
                 });
        }
    }

    public class TianaJoiasSchema : Schema
    {
        public TianaJoiasSchema(IServiceProvider serviceProvider)
        : base(serviceProvider)
        {
            Query = serviceProvider.GetRequiredService<ProductQuery>();
           
        }
    }
}
