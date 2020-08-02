using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WebApi.Domain;

namespace WebApi.Infra.EFMappers
{
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
}
