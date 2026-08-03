using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SystemDistributedOrders.Domain.Entities;
using SystemDistributedOrders.Infrastructure.Persistence;
using SystemDistributedOrders.Infrastructure.Persistence.Repositories;

namespace SystemDistributedOrders.Infrastructure.Tests.Persistence;

public sealed class ProductCatalogTests
{
    [Fact]
    public async Task GetAll_ShouldReturnProductsOrderedByName()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync(TestContext.Current.CancellationToken);

        var options = new DbContextOptionsBuilder<OrdersDbContext>()
            .UseSqlite(connection)
            .Options;

        await using var context = new OrdersDbContext(options);
        await context.Database.EnsureCreatedAsync(TestContext.Current.CancellationToken);

        context.Products.AddRange(
            new Product("Teclado", 200m),
            new Product("Mouse", 100m));
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var products = await new ProductCatalog(context)
            .GetAllAsync(TestContext.Current.CancellationToken);

        Assert.Equal(["Mouse", "Teclado"], products.Select(product => product.Name));
    }

    [Fact]
    public async Task GetById_WithUnknownId_ShouldReturnNull()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync(TestContext.Current.CancellationToken);

        var options = new DbContextOptionsBuilder<OrdersDbContext>()
            .UseSqlite(connection)
            .Options;

        await using var context = new OrdersDbContext(options);
        await context.Database.EnsureCreatedAsync(TestContext.Current.CancellationToken);

        var product = await new ProductCatalog(context).GetByIdAsync(
            Guid.NewGuid(),
            TestContext.Current.CancellationToken);

        Assert.Null(product);
    }
}
