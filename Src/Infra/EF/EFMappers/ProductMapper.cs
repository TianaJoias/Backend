using System;
using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.EF.EFMappers
{
    internal class ProductMapper : EntityMapper<Product>
    {
        public override void Configure(EntityTypeBuilder<Product> builder)
        {
            base.Configure(builder);
            //var converter = new ValueConverter<IList<int>, string>(
            //    v => string.Join(";", v),
            //    v => (v ?? "").Split(";".ToArray(), StringSplitOptions.RemoveEmptyEntries).Select(val => int.Parse(val)).ToList());

            builder.ToTable("Product");
            builder.Property(x => x.BarCode);
            builder.Property(x => x.Description);
            builder.HasMany(x => x.Categories).WithOne(it => it.Product);

        }
    }

    internal class CategoryMapper : IEntityTypeConfiguration<ProductCategory>
    {
        public void Configure(EntityTypeBuilder<ProductCategory> builder)
        {
            builder.ToTable("ProductCategory");
            builder.HasKey(x => new { x.ProductId, x.TagId });
        }
    }

    internal class TagMapper : EntityMapper<Tag>
    {
        public override void Configure(EntityTypeBuilder<Tag> builder)
        {
            base.Configure(builder);
            builder.ToTable("Tags");
            builder.Property(x => x.Name);
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
            builder.HasMany(x => x.Suppliers).WithMany(x => x.Batchs);
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
