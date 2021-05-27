using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace WebApi.Extensions
{
    public static class CorsExtensions
    {
        public static IServiceCollection AddCorsCustom(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("mypolicy",
                    builder => builder
                    .AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod());
            });
            return services;
        }

        public static void UserCors(IApplicationBuilder app)
        {
            app.UseCors("mypolicy");
        }
    }
}
