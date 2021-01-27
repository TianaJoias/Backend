using System;
using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infra.EF
{
    public static class DataBaseExtensions
    {
        public const string SQLITE_STRATEGY = "SQLITE";
        public const string COSMOS_STRATEGY = "COSMOS";
        public const string NPGSQL_STRATEGY = "NPGSQL";
        public static IServiceCollection AddSqlLite(this IServiceCollection services, IConfiguration Configuration)
        {
            //https://www.treinaweb.com.br/blog/utilizando-o-nhibernate-em-uma-aplicacao-asp-net-core/
            services.Scan(scan => scan
                 .FromAssemblyOf<TianaJoiasContextDB>()
                .AddClasses(classes => classes.AssignableTo(typeof(IRepository<>)))
                .AsSelfWithInterfaces().WithScopedLifetime());

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddDbContextPool<TianaJoiasContextDB>(options => UseDatabase(options, Configuration));
            return services;
        }

        private static DbContextOptionsBuilder UseDatabase(DbContextOptionsBuilder options, IConfiguration configuration)
        {
            var dbStrategy = configuration.GetSection("DBStrategy").Value;
            dbStrategy = dbStrategy.ToUpper();
            string connectionString;
            switch (dbStrategy)
            {
                case NPGSQL_STRATEGY:
                    connectionString = configuration.GetConnectionString(NPGSQL_STRATEGY);
                    return options.UseNpgsql(connectionString);
                case SQLITE_STRATEGY:
                    connectionString = configuration.GetConnectionString(SQLITE_STRATEGY);
                    return options.UseSqlite(connectionString);
                case COSMOS_STRATEGY:
                    connectionString = configuration.GetConnectionString(COSMOS_STRATEGY);
                    return options.UseCosmos(connectionString, "TianaJoias");
                default:
                    var argumentOutOfRangeException = new ArgumentOutOfRangeException("DBStrategy", dbStrategy, "DB Strategy not found.");
                    throw argumentOutOfRangeException;
            }
        }
    }
}

