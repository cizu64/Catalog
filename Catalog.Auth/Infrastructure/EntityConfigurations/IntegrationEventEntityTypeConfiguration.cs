using Catalog.Auth.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Auth.Infrastructure.EntityConfigurations
{
    public class IntegrationEventEntityTypeConfiguration : IEntityTypeConfiguration<IntegrationEvent>
    {
        public void Configure(EntityTypeBuilder<IntegrationEvent> builder)
        {
            builder.ToTable("IntegrationEvent");
            builder.HasKey(i => i.Id);
            builder.Property(i => i.Id).IsRequired();
            builder.Property(i => i.IsPublished).IsRequired();
            builder.Property(i => i.Queue).IsRequired();
        }
    }
}
