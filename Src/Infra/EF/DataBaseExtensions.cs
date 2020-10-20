using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infra.EF
{
    public static class DataBaseExtensions
    {
        public static IServiceCollection AddSqlLite(this IServiceCollection services, IConfiguration Configuration)
        {
            //https://www.treinaweb.com.br/blog/utilizando-o-nhibernate-em-uma-aplicacao-asp-net-core/
            var connectionString = Configuration.GetConnectionString(TianaJoiasContextDB.SqlLiteConnectionName);
            services.Scan(scan => scan
                 .FromAssemblyOf<TianaJoiasContextDB>()
                .AddClasses(classes => classes.AssignableTo(typeof(IRepository<>)))
                .AsSelfWithInterfaces().WithScopedLifetime());

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddDbContextPool<TianaJoiasContextDB>(options => options.UseSqlite(connectionString));
            return services;
        }
    }
}

