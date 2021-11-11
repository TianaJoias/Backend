using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Infra.EF.EFMappers.Portifolio;
using Domain.Portifolio;
using Domain.Account;
using Domain.Stock;
using Domain.Catalog;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Infra.EF
{
    public sealed class ProductContextDB : DbContext
    {
        public ProductContextDB(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProductMapper).Assembly);
        }

        public async Task Seeding(IPasswordService passwordService)
        {

            var logger = this.GetInfrastructure().GetRequiredService<ILoggerFactory>();
            try
            {
                Database.Migrate();
                var guid = Guid.Parse("{0963682F-4DBC-4827-B500-B7F45A6345C3}");
                var adminAccount = await Set<Account>().FirstOrDefaultAsync(b => b.Id == guid);
                if (adminAccount is null)
                {
                    await AddAccount(this, passwordService, guid);
                    await AddTags(this);
                    await AddSupplier(this);
                    await SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                logger.CreateLogger<ProductContextDB>().LogError(ex, "Entity Framework migration error");
            }
        }

        private static async Task AddSupplier(ProductContextDB context)
        {
            var supplier = new Supplier
            {
                Id = Guid.Parse("{B4193BD2-5753-49E3-9850-D13FE9CDE43E}"),
                Description = "Supplier ONE",
                Name = "Supplier One",
            };
            await context.Set<Supplier>().AddRangeAsync(supplier);
        }

        private static async Task AddTags(ProductContextDB context)
        {
            var tags = new List<Tag> {
                new Tag("Anel", Tag.TagType.Group),
                new Tag("Pulseiras", Tag.TagType.Group),
                new Tag("Brincos", Tag.TagType.Group),
                new Tag("Infanto", Tag.TagType.Gender),
                new Tag("Masculino", Tag.TagType.Gender),
                new Tag("Feminino", Tag.TagType.Gender),
                new Tag("Unissex", Tag.TagType.Gender),
                new Tag("Aço", Tag.TagType.Typology),
                new Tag("Prata", Tag.TagType.Typology),
                new Tag("Semijoia", Tag.TagType.Typology),
                new Tag("Pronta", Tag.TagType.Plated),
                new Tag("Galvanizada", Tag.TagType.Plated),
                new Tag("Ouro", Tag.TagType.Color),
                new Tag("Ouro Rose", Tag.TagType.Color),
                new Tag("Ródio Negro", Tag.TagType.Color),
                new Tag("Ródio Branco", Tag.TagType.Color),
            };
            await context.Set<Tag>().AddRangeAsync(tags);
        }

        private static async Task AddAccount(ProductContextDB context, IPasswordService passwordService, Guid guid)
        {
            var password = await passwordService.Hash("ADMIN");
            var account = new Account
            {
                Id = guid,
                Roles = new List<Roles> { Roles.ADMIN },
                User = new User
                {
                    Email = "admin@tianajoias.com.br",
                    Password = password
                }
            };
            await context.Set<Account>().AddAsync(account);
            var channel = new Agent(guid, guid);
            await context.Set<Agent>().AddAsync(channel);
        }
    }
}
