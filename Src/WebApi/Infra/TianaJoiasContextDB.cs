using Microsoft.EntityFrameworkCore;
using WebApi.Domain;
using WebApi.Infra.EFMappers;

namespace WebApi.Infra
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
