using System.Text.Json;
using Domain;
using Domain.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Infra.EF.EFMappers.Catalog
{
    internal class AgentMapper : EntityMapper<Agent>
    {
        public override void Configure(EntityTypeBuilder<Agent> builder)
        {
            base.Configure(builder);

            builder.ToTable("Agents");
            builder.Property(it => it.OwnerId).IsRequired();
            builder.Property(it => it.AccountableId).IsRequired();
            builder.HasMany<Domain.Catalog.Catalog>().WithOne(it => it.Channel).IsRequired();
        }
    }
    internal class CatalogMapper : EntityMapper<Domain.Catalog.Catalog>
    {
        public override void Configure(EntityTypeBuilder<Domain.Catalog.Catalog> builder)
        {
            base.Configure(builder);
            builder.ToTable("Catalogs");
            builder.Property(it => it.Closed);
            builder.Property(it => it.Opened);
            builder.Property(it => it.TotalSold);
            builder.HasOne(it => it.Channel).WithMany();
            builder.HasMany(it => it.Items).WithOne().HasForeignKey("CatalogId").HasConstraintName("FK_CATALOG_ITEM");
            builder.Metadata
                .FindNavigation("Items")
                .SetPropertyAccessMode(PropertyAccessMode.Field);
        }
    }

    internal class CatalogItemsMapper : EntityMapper<CatalogItem>
    {
        public override void Configure(EntityTypeBuilder<CatalogItem> builder)
        {

            base.Configure(builder);

            builder.ToTable("CatalogItems");
            builder.Property(it => it.EAN);
            builder.Property(it => it.Enabled);
            builder.Property(it => it.LongDescription);
            builder.Property(it => it.LotId);
            builder.Property(it => it.Price);
            builder.Property(it => it.ProdutoId);
            builder.Property(it => it.InitialQuantity);
            builder.Property(it => it.CurrentQuantity);
            builder.Property(it => it.ShortDescription);
            builder.Property(it => it.SKU);
            builder.Property(it => it.TotalSold);
            builder.Property(it => it.Thumbnail).HasJsonConversion();
        }
    }

    public static class ValueConversionExtensions
    {
        public static PropertyBuilder<T> HasJsonConversion<T>(this PropertyBuilder<T> propertyBuilder)
        {
            ValueConverter<T, string> converter = new ValueConverter<T, string>(
                v => JsonSerializer.Serialize(v, null),
                v => JsonSerializer.Deserialize<T>(v, null));

            ValueComparer<T> comparer = new ValueComparer<T>(
                (l, r) => JsonSerializer.Serialize(l, null) == JsonSerializer.Serialize(r, null),
                v => v == null ? 0 : JsonSerializer.Serialize(v, null).GetHashCode(),
                v => JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(v, null), null));

            propertyBuilder.HasConversion(converter);
            propertyBuilder.Metadata.SetValueConverter(converter);
            propertyBuilder.Metadata.SetValueComparer(comparer);

            return propertyBuilder;
        }
    }
}
