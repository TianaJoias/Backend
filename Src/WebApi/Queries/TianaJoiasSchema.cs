using System;
using GraphQL.Authorization;
using GraphQL.Types;
using Microsoft.Extensions.DependencyInjection;

namespace WebApi.Queries
{
    public class TianaJoiasSchema : Schema
    {
        public TianaJoiasSchema(IServiceProvider serviceProvider)
        : base(serviceProvider)
        {
            Query = serviceProvider.GetService<ProductQuery>();
            Query.AuthorizeWith("AdminPolicy");
        }
    }
}
