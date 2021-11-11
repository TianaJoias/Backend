using Domain.Products.Write;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;

namespace Infra.Products.Mappers
{
    internal class VariantMapper : EntityMapper<Variant>
    {
        public override void Configure(EntityTypeBuilder<Variant> builder)
        {
            base.Configure(builder);
            builder.ToTable("Variants");
            builder.Property(it => it.Title);
            builder.Property(it => it.UpdateAt);
            builder.Property(it => it.CreateAt);
            builder.HasOne(it => it.Product);
            builder.Ignore(it => it.Images);
        }
    }
    public static class ValueConversionExtensions
    {
        public static PropertyBuilder<T> HasJsonConversion<T>(this PropertyBuilder<T> propertyBuilder) where T : class
        {
            var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto };

            ValueConverter<T, string> converter = new ValueConverter<T, string>
            (
                v => JsonConvert.SerializeObject(v, typeof(T), settings),
                v => JsonConvert.DeserializeObject<T>(v, settings) ?? null
            );

            ValueComparer<T> comparer = new ValueComparer<T>
            (
                (l, r) => JsonConvert.SerializeObject(l, typeof(T), settings) == JsonConvert.SerializeObject(r, settings),
                v => v == null ? 0 : JsonConvert.SerializeObject(v, typeof(T), settings).GetHashCode(),
                v => JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(v, typeof(T), settings), settings)
            );

            propertyBuilder.HasConversion(converter);
            propertyBuilder.Metadata.SetValueConverter(converter);
            propertyBuilder.Metadata.SetValueComparer(comparer);
            propertyBuilder.HasColumnType("jsonb");

            return propertyBuilder;
        }
    }
}
