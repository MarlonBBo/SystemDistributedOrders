using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SystemDistributedOrders.Domain.Entities;

namespace SystemDistributedOrders.Infrastructure.Persistence.Configurations;

internal sealed class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");
        builder.HasKey(order => order.Id);
        builder.Property(order => order.Id).ValueGeneratedNever();

        builder.Property(order => order.CustomerId).IsRequired();
        builder.Property(order => order.Status).HasConversion<int>().IsRequired();
        builder.Property(order => order.CreatedAt).HasPrecision(3);
        builder.Property(order => order.UpdatedAt).HasPrecision(3);

        builder.Ignore(order => order.Total);

        builder.HasMany(order => order.Items)
            .WithOne()
            .HasForeignKey(item => item.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(order => order.Items)
            .HasField("_item")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasIndex(order => order.CustomerId);
        builder.HasIndex(order => order.Status);
    }
}
