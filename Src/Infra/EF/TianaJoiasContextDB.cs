using System;
using System.Collections.Generic;
using Domain;
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
        public static string SqlLiteConnectionName = "TianaJoiasConnectionString-SqlLite";
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
                await AddProducts(context);
                await Lot(context);
                await context.SaveChangesAsync();
            }
        }

        private static async Task Lot(TianaJoiasContextDB context)
        {
            var supplier = new Supplier
            {
                Id = Guid.Parse("{B4193BD2-5753-49E3-9850-D13FE9CDE43E}"),
                Description = "Supplier ONE",
                Name = "Supplier One",
            };
            await context.Set<Supplier>().AddRangeAsync(supplier);
            var lot = new Lot
            {
                CostPrice = 10,
                Number = "123",
                Weight = 10,
                ProductId = Guid.Parse("{37F1AD8B-5707-4C2E-BEB6-BAF05BF18E9C}"),
                Quantity = 10,
                SalePrice = 30,
                EAN = "1-2o3809123",
                Suppliers = new List<Supplier> { supplier },
                Date = Clock.Now,
                Id = Guid.Parse("{37F1AD8B-5707-4C2E-BEB6-BAF05BF18E9C}"),
            };
            await context.Set<Lot>().AddRangeAsync(lot);
        }

        private static async Task AddProducts(TianaJoiasContextDB context)
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

            var firstProduct = new Product
            {
                Id = Guid.Parse("{37F1AD8B-5707-4C2E-BEB6-BAF05BF18E9C}"),
                EAN = "123456",
                Description = "First Product"
            };
            var secondProduct = new Product
            {
                Id = Guid.Parse("{C6504702-A0B6-4D95-9B9B-8A417316A15D}"),
                EAN = "654321",
                Description = "second Product"
            };

            firstProduct.AddCategory(firstTag);
            secondProduct.AddCategory(secondTag);

            await context.Set<Product>().AddRangeAsync(firstProduct, secondProduct);
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
            var channel = new Agent
            {
                OwnerId = guid,
                AccountableId = guid,
                Id = Guid.Parse("{498B49C8-B17D-4A40-BCC1-EEA97508A344}")
            };
            await context.Set<Agent>().AddAsync(channel);
        }
    }
}
