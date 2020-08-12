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
            builder.Property(x => x.BarCode);
            builder.Property(x => x.Description);
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
    internal class BatchMapper : EntityMapper<Batch>
    {
        public override void Configure(EntityTypeBuilder<Batch> builder)
        {
            base.Configure(builder);

            builder.ToTable("Batch");
            builder.HasOne(it => it.Product).WithOne().HasForeignKey<Batch>();
            builder.Property(x => x.Number);
            builder.Property(x => x.Weight);
            builder.Property(x => x.Quantity);
            builder.Property(x => x.Date);
            builder.Property(x => x.SaleValue);
            builder.Property(x => x.CostValue);
            builder.HasMany(x => x.Suppliers).WithOne();
        }
    }

    internal class SupplierMapper : EntityMapper<Supplier>
    {
        public override void Configure(EntityTypeBuilder<Supplier> builder)
        {
            base.Configure(builder);
            builder.ToTable("Supplier");
            builder.Property(x => x.Description);
            builder.Property(x => x.Name);
        }
    }
}
