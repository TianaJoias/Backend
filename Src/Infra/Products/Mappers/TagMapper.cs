using Domain.Products.Write;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.Products.Mappers
{
    internal class TagMapper : EntityMapper<Tag>
    {
        public override void Configure(EntityTypeBuilder<Tag> builder)
        {
            base.Configure(builder);
            builder.ToTable("Tags");
            builder.Property(it => it.Title);
            builder.Property(it => it.UpdateAt);
            builder.Property(it => it.CreateAt);
        }
    }
}
