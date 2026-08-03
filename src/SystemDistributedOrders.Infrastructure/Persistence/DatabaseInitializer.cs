using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SystemDistributedOrders.Domain.Entities;

namespace SystemDistributedOrders.Infrastructure.Persistence;

public static class DatabaseInitializer
{
    private static readonly (string Name, decimal Price)[] InitialProducts =
    [
        ("Notebook Pro 15", 6499.90m),
        ("Monitor UltraWide 29", 1899.90m),
        ("Teclado Mecânico", 429.90m),
        ("Mouse Sem Fio", 189.90m),
        ("Headset Gamer", 349.90m),
        ("Webcam Full HD", 279.90m),
        ("SSD NVMe 1 TB", 599.90m),
        ("Hub USB-C", 239.90m),
        ("Cadeira Ergonômica", 1599.90m),
        ("Smartphone 5G", 2799.90m)
    ];

    public static async Task InitializeDatabaseAsync(
        this IServiceProvider services,
        CancellationToken cancellationToken = default)
    {
        await using var scope = services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<OrdersDbContext>();

        await context.Database.MigrateAsync(cancellationToken);

        var existingNames = await context.Products
            .Select(product => product.Name)
            .ToHashSetAsync(cancellationToken);

        var missingProducts = InitialProducts
            .Where(product => !existingNames.Contains(product.Name))
            .Select(product => new Product(product.Name, product.Price));

        await context.Products.AddRangeAsync(missingProducts, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }
}
