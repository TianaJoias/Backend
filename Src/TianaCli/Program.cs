using System;
using Infra.Application;
using Infra.Products;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace TianaCli
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }
    }
    public class ProductContextDBFactory : IDesignTimeDbContextFactory<ProductContextDB>
    {
        private IServiceProvider GetProvider()
        {
            return new ServiceCollection()
             .AddLogging()
             .AddMediatR(typeof(Program))
             .BuildServiceProvider();
        }

        public ProductContextDB CreateDbContext(string[] args)
        {
            var provider = GetProvider();
            var optionsBuilder = new DbContextOptionsBuilder<ProductContextDB>();
            optionsBuilder.UseSqlite("Data Source=.\\Products.db");

            return new ProductContextDB(optionsBuilder.Options, provider.GetRequiredService<IMediator>(), provider.GetRequiredService<ILogger<ProductContextDB>>());
        }

    }

    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationContextDB>
    {
        private IServiceProvider GetProvider()
        {
            return new ServiceCollection()
             .AddLogging()
             .AddMediatR(typeof(Program))
             .BuildServiceProvider();
        }
        public ApplicationContextDB CreateDbContext(string[] args)
        {
            var provider = GetProvider();
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationContextDB>();
            optionsBuilder.UseSqlite("Data Source=.\\Application.db");

            return new ApplicationContextDB(optionsBuilder.Options, provider.GetRequiredService<ILogger<ApplicationContextDB>>());
        }
    }
}


