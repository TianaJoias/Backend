using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Infra.EF.EFMappers.Portifolio;
using Domain.Portifolio;
using Domain.Account;
using Domain.Stock;
using Domain.Catalog;

namespace Infra.EF
{
    public sealed class TianaJoiasContextDB : DbContext
    {
        public TianaJoiasContextDB(DbContextOptions options) : base(options) { }

        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProductMapper).Assembly);
        }

        public static async Task Seeding(TianaJoiasContextDB context, IPasswordService passwordService)
        {
            context.Database.Migrate();
            var guid = Guid.Parse("{0963682F-4DBC-4827-B500-B7F45A6345C3}");
            var adminAccount = await context.Set<Account>().FirstOrDefaultAsync(b => b.Id == guid);
            if (adminAccount is null)
            {
                await AddAccount(context, passwordService, guid);
                await AddTags(context);
                await AddSupplier(context);
                await context.SaveChangesAsync();
            }
        }

        private static async Task AddSupplier(TianaJoiasContextDB context)
        {
            var supplier = new Supplier
            {
                Id = Guid.Parse("{B4193BD2-5753-49E3-9850-D13FE9CDE43E}"),
                Description = "Supplier ONE",
                Name = "Supplier One",
            };
            await context.Set<Supplier>().AddRangeAsync(supplier);
        }

        private static async Task AddTags(TianaJoiasContextDB context)
        {
            var firstTag = new Tag
            {
                Name = "firstTag"
            };
            var secondTag = new Tag
            {
                Name = "secondTag"
            };
            await context.Set<Tag>().AddRangeAsync(firstTag, secondTag);        
        }

        private static async Task AddAccount(TianaJoiasContextDB context, IPasswordService passwordService, Guid guid)
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
