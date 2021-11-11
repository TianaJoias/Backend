using System.Collections.Generic;
using Domain.Products.Write;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.Products.Mappers
{
    internal class ProductMapper : EntityMapper<Product>
    {
        public override void Configure(EntityTypeBuilder<Product> builder)
        {
            base.Configure(builder);
            builder.ToTable("Products");
            builder.Property(it => it.Title);
            builder.Property(it => it.HtmlBody);
            builder.Property(it => it.UpdateAt);
            builder.Property(it => it.CreateAt);
            builder.Property(it => it.CorrelationId);
            //builder.Property(it => it.Categories).HasField("_categories")
            //   .UsePropertyAccessMode(PropertyAccessMode.PreferField);
            //builder.HasMany(it => it.Categories).WithOne();


            //builder.Property(it => it.Tags).HasField("_tags")
            //   .UsePropertyAccessMode(PropertyAccessMode.PreferField);
            builder.HasMany(it => it.Tags).WithOne().Metadata.PrincipalToDependent.SetPropertyAccessMode(PropertyAccessMode.Field);

            builder
                .HasMany(p => p.Categories)
                .WithMany(p => p.Products)
                .UsingEntity<Dictionary<string, object>>(
                    "ProductCategory",
                    j => j
                        .HasOne<Category>()
                        .WithMany()
                        .HasForeignKey("CategoryId")
                        .HasConstraintName("FK_ProductCategory_Category_CategoryId")
                        .OnDelete(DeleteBehavior.Cascade),
                    j => j
                        .HasOne<Product>()
                        .WithMany()
                        .HasForeignKey("ProductId")
                        .HasConstraintName("FK_ProductCategory_Product_ProductId")
                        .OnDelete(DeleteBehavior.ClientCascade));
            //builder.Property(p => p.Categories).HasField("_categories")
            //   .UsePropertyAccessMode(PropertyAccessMode.PreferField);
         //   var elementMetadata = builder.Metadata.GetDeclaredNavigations();//.FindNavigation(nameof(Product.Categories));
            //elementMetadata.SetPropertyAccessMode(PropertyAccessMode.Property);
        }
    }
}
