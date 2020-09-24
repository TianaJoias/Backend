using Domain;
using Infra.EF.EFMappers;
using Microsoft.EntityFrameworkCore;

namespace Infra.EF
{
    public sealed class TianaJoiasContextDB : DbContext
    {
        public static string SqlLiteConnectionName = "TianaJoiasConnectionString-SqlLite";
        public TianaJoiasContextDB(DbContextOptions options) : base(options) { }

        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProductMapper).Assembly);
        }
    }
}
