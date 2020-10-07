using System.Text.Json;
using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Infra.EF.EFMappers
{
    internal class ChannelMapper : EntityMapper<Channel>
    {
        public override void Configure(EntityTypeBuilder<Channel> builder)
        {
            base.Configure(builder);

            builder.ToTable("Channel");
            builder.Property(it => it.AccountOwnerId);
            builder.HasOne(x => x.CurrentCatalog);
            builder.HasMany(x => x.HistoryCatalogs).WithOne().HasForeignKey("ChannelId").HasConstraintName("FK_CHANNEL");
        }
    }
    internal class CatalogMapper : EntityMapper<Catalog>
    {
        public override void Configure(EntityTypeBuilder<Catalog> builder)
        {
            base.Configure(builder);

            builder.ToTable("Catalog");
            builder.Property(it => it.Closed);
            builder.Property(it => it.Opened);
            builder.HasMany(it => it.Items).WithOne().HasForeignKey("CatalogId").HasConstraintName("FK_CATALOG_ITEM");
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
            builder.Property(it => it.Quantity);
            builder.Property(it => it.ShortDescription);
            builder.Property(it => it.SKU);
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
