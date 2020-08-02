using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WebApi.Domain;
using WebApi.Infra.Repositories;

namespace WebApi.Infra
{
    public static class DataBaseExtensions
    {
        public static IServiceCollection AddSqlLite(this IServiceCollection services, IConfiguration Configuration)
        {
            var connectionString = Configuration.GetConnectionString(TianaJoiasContextDB.SqlLiteConnectionName);
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddDbContextPool<TianaJoiasContextDB>(options => options.UseSqlite(connectionString));
            return services;
        }
    }
}

