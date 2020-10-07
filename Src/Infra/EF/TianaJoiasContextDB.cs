using System;
using System.Collections.Generic;
using Domain;
using Infra.EF.EFMappers;
using Microsoft.EntityFrameworkCore;
using System.Linq;

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

        public static void Seeding(TianaJoiasContextDB context)
        {
            context.Database.Migrate();
            var guid = Guid.Parse("{0963682F-4DBC-4827-B500-B7F45A6345C3}");
            var testBlog = context.Set<Account>().FirstOrDefault(b => b.Id == guid);
            if (testBlog is null)
            {
                var password = BCrypt.Net.BCrypt.HashPassword("ADMIN", 11);
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
                context.Set<Account>().Add(account);
                context.SaveChanges();
            }
        }
    }
}
