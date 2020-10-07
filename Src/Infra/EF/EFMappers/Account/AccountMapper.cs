using System;
using System.Collections.Generic;
using System.Linq;
using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Infra.EF.EFMappers
{
    internal class AccountMapper : EntityMapper<Account>
    {
        public override void Configure(EntityTypeBuilder<Account> builder)
        {
            var converter = new ValueConverter<IEnumerable<Roles>, string>(
                v => string.Join(";", v),
                v => (v ?? "").Split(";".ToArray(), StringSplitOptions.RemoveEmptyEntries).Select(val => Enum.Parse<Roles>(val)));

            base.Configure(builder);
            builder.ToTable("Accounts");
            builder.OwnsOne(x => x.User);
            builder.Property(x => x.Roles).HasConversion(converter);
            builder.HasMany(it => it.ExternalProviders).WithOne();
        }
    }
}
