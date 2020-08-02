using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace WebApi.Infra
{
    public class TianaJoiasContextDBFactory : IDesignTimeDbContextFactory<TianaJoiasContextDB>
    {
        public TianaJoiasContextDB CreateDbContext(string[] args)
        {
            string environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            // Build config
            IConfiguration config = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory()))
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment}.json", optional: true)
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<TianaJoiasContextDB>();
            optionsBuilder.UseSqlite(config.GetConnectionString(TianaJoiasContextDB.SqlLiteConnectionName));
            return new TianaJoiasContextDB(optionsBuilder.Options);
        }
    }
}
