using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Infra.EF.EFMappers.Account
{
    internal class AccountMapper : EntityMapper<Domain.Account.Account>
    {
        public override void Configure(EntityTypeBuilder<Domain.Account.Account> builder)
        {
            var converter = new ValueConverter<IEnumerable<Domain.Account.Roles>, string>(
                v => string.Join(";", v),
                v => (v ?? "").Split(";".ToArray(), StringSplitOptions.RemoveEmptyEntries).Select(val => Enum.Parse<Domain.Account.Roles>(val)));

            base.Configure(builder);
            builder.ToTable("Accounts");
            builder.Property(x => x.Name);
            builder.OwnsOne(x => x.User);
            builder.OwnsOne(x => x.Address);
            builder.Property(x => x.Roles).HasConversion(converter);
            builder.HasMany(it => it.ExternalProviders).WithOne();
        }
    }
}
