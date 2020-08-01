using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Configuration;
using WebApi.Controllers;

namespace WebApi.Infra
{
    public interface IUnitOfWork
    {
        Task<bool> Commit();
        Task Add<T>(T entity) where T : class, IEntity;
        Task Remove<T>(T entity) where T : class, IEntity;
        Task Update<T>(T entity) where T : class, IEntity;
    }

    public sealed class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly TianaJoiasContextDB _context;
        public UnitOfWork(TianaJoiasContextDB context)
        {
            _context = context;
        }

        private bool _disposed = false;


        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public async Task<bool> Commit()
        {
            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task Add<T>(T entity) where T : class, IEntity
        {
            await _context.Set<T>().AddAsync(entity);
        }

        public Task Remove<T>(T entity) where T : class, IEntity
        {
            _context.Set<T>().Remove(entity);
            return Task.CompletedTask;
        }

        public Task Update<T>(T entity) where T : class, IEntity
        {
            _context.Set<T>().Update(entity);
            return Task.CompletedTask;
        }
    }
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
    public class UserContextDBFactory : IDesignTimeDbContextFactory<TianaJoiasContextDB>
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
    internal class ProductMapper : EntityMapper<Product>
    {
        public override void Configure(EntityTypeBuilder<Product> builder)
        {
            base.Configure(builder);
            var converter = new ValueConverter<IList<int>, string>(
                v => string.Join(";", v),
                v => (v ?? "").Split(";", StringSplitOptions.RemoveEmptyEntries).Select(val => int.Parse(val)).ToList());

            builder.ToTable("Product");
            builder.Property(x => x.SalePrice);
            builder.Property(x => x.CostValue);
            builder.Property(x => x.BarCode);
            builder.Property(x => x.Description);
            builder.Property(x => x.Quantity);
            builder.Property(x => x.Weight);
            builder.Property(x => x.Supplier);
            builder.Property(x => x.Colors)
                .HasConversion(converter);
            builder.Property(x => x.Typologies)
                .HasConversion(converter);
            builder.Property(x => x.Thematics)
                .HasConversion(converter);
            builder.Property(x => x.Categories)
                .HasConversion(converter);

        }
    }
    internal abstract class EntityMapper<T> : IEntityTypeConfiguration<T> where T : BaseEntity
    {
        public virtual void Configure(EntityTypeBuilder<T> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnName("Id").ValueGeneratedOnAdd();
        }
    }
}
