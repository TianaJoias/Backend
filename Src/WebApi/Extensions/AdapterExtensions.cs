using Microsoft.AspNetCore.Builder;

namespace WebApi.Extensions
{
    public static class AdapterExtensions
    {
        public static void UserTypeAdapter(this IApplicationBuilder app)
        {
            //TypeAdapterConfig<Product, ProductQueryResult>.NewConfig()
            //    .Map(dest => dest.Tags, src => src.Tags.Select(it => it.Id));
        }
    }
}
