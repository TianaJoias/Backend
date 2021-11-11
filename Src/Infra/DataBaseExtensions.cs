using System;
using System.Reflection;
using System.Threading.Tasks;
using Application;
using Infra.Application;
using Infra.Products;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infra
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
                 .FromAssemblyOf<ProductContextDB>()
                .AddClasses(classes => classes.AssignableTo(typeof(IRepositoryWrite<>)))
                .AsSelfWithInterfaces().WithScopedLifetime());

            services.AddScoped<IUnitOfWork, ProductContextDB>();
            services.AddDbContext<ProductContextDB>((serviceProvider, optionsBuilder) =>
            {
                UseDatabaseProduts(optionsBuilder, Configuration);
            }, ServiceLifetime.Scoped);

            services.AddDbContext<ApplicationContextDB>((serviceProvider, optionsBuilder) =>
            {
                UseDatabaseEntity(optionsBuilder, Configuration);

            }, ServiceLifetime.Scoped);
            return services;
        }

        public static void UseEF(this IApplicationBuilder app, IServiceScopeFactory serviceScopeFactory)
        {
            Task.Run(async () =>
            {
                using var scope = serviceScopeFactory.CreateScope();
                var serviceProvider = scope.ServiceProvider;
                var dataContext = serviceProvider.GetService<ProductContextDB>();
                await dataContext.Seeding();
            });
        }

        public static DbContextOptionsBuilder UseDatabaseEntity(DbContextOptionsBuilder options, IConfiguration configuration)
        {
            var dbStrategy = configuration.GetSection("Contexts").GetSection("Application").GetSection("Strategy").Value;
            dbStrategy = dbStrategy.ToUpper();
            var connectionString = configuration.GetSection("Contexts").GetSection("Application").GetSection("ConectionString").Value;
            //var builder = options.UseInternalServiceProvider(serviceProvider);
            switch (dbStrategy)
            {
                case NPGSQL_STRATEGY:
                    return options.UseNpgsql(connectionString);
                case SQLITE_STRATEGY:
                    return options.UseSqlite(connectionString, options => { 
                        options.MigrationsAssembly(typeof(ApplicationContextDB).GetTypeInfo().Assembly.GetName().Name);
                    });
                default:
                    var argumentOutOfRangeException = new ArgumentOutOfRangeException("DBStrategy", dbStrategy, "DB Strategy not found.");
                    throw argumentOutOfRangeException;
            }
        }
        public static DbContextOptionsBuilder UseDatabaseProduts(DbContextOptionsBuilder options, IConfiguration configuration)
        {
            var dbStrategy = configuration.GetSection("Contexts").GetSection("Products").GetSection("Strategy").Value;
            dbStrategy = dbStrategy.ToUpper();
            var connectionString = configuration.GetSection("Contexts").GetSection("Products").GetSection("ConectionString").Value;
            switch (dbStrategy)
            {
                case NPGSQL_STRATEGY:
                    return options.UseNpgsql(connectionString);
                case SQLITE_STRATEGY:
                    return options.UseSqlite(connectionString, options => {
                        options.MigrationsAssembly(typeof(ProductContextDB).GetTypeInfo().Assembly.GetName().Name);
                    });
                default:
                    var argumentOutOfRangeException = new ArgumentOutOfRangeException("DBStrategy", dbStrategy, "DB Strategy not found.");
                    throw argumentOutOfRangeException;
            }
        }
    }
}

