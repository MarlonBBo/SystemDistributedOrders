using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SystemDistributedOrders.Domain.Entities;

namespace SystemDistributedOrders.Infrastructure.Persistence.Configurations;

internal sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");
        builder.HasKey(product => product.Id);
        builder.Property(product => product.Id).ValueGeneratedNever();

        builder.Property(product => product.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(product => product.Price)
            .HasPrecision(18, 2);

        builder.Property(product => product.CreatedAt)
            .HasPrecision(3);

        builder.Property(product => product.UpdatedAt)
            .HasPrecision(3);
    }
}
