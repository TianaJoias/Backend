﻿using System.Linq;
using Domain.Portifolio;
using GraphQL.Types;

namespace WebApi.Queries
{
    public class ProductType : ObjectGraphType<Product>
    {
        public ProductType()
        {
            Name = "Product";
            Field(x => x.Id, type: typeof(GuidGraphType)).Description("Product Id");
            Field(x => x.SKU).Description("Sku of Product");
            Field(x => x.Description).Description("Description of product");
            Field<ListGraphType<GuidGraphType>>("Categories", description: "Categories of the product", resolve: context =>
            {
                return context.Source.Categories.Select(it => it.Id);
            });
        }
    }
}
