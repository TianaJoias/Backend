using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra
{
    internal abstract class EntityMapper<T> : IEntityTypeConfiguration<T> where T : BaseEntity
    {
        public virtual void Configure(EntityTypeBuilder<T> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Ignore(X => X.Events);
            builder.Property(x => x.Id).HasColumnName("Id").ValueGeneratedOnAdd();
        }
    }
}
