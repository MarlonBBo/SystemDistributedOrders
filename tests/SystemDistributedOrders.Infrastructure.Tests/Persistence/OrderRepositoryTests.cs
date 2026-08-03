using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SystemDistributedOrders.Domain.Entities;
using SystemDistributedOrders.Infrastructure.Persistence;
using SystemDistributedOrders.Infrastructure.Persistence.Repositories;

namespace SystemDistributedOrders.Infrastructure.Tests.Persistence;

public sealed class OrderRepositoryTests
{
    [Fact]
    public async Task AddAndGetById_ShouldPersistOrderWithItems()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync(TestContext.Current.CancellationToken);

        var options = new DbContextOptionsBuilder<OrdersDbContext>()
            .UseSqlite(connection)
            .Options;

        await using var context = new OrdersDbContext(options);
        await context.Database.EnsureCreatedAsync(TestContext.Current.CancellationToken);

        var order = new Order(Guid.NewGuid());
        order.AddItem(Guid.NewGuid(), "Produto de teste", 25.50m, 2);

        var repository = new OrderRepository(context);
        await repository.AddAsync(order, TestContext.Current.CancellationToken);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);
        context.ChangeTracker.Clear();

        var persistedOrder = await repository.GetByIdAsync(
            order.Id,
            TestContext.Current.CancellationToken);

        Assert.NotNull(persistedOrder);
        Assert.Single(persistedOrder.Items);
        Assert.Equal(51.00m, persistedOrder.Total);
    }

    [Fact]
    public async Task GetAll_ShouldReturnNewestOrderFirst()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync(TestContext.Current.CancellationToken);

        var options = new DbContextOptionsBuilder<OrdersDbContext>()
            .UseSqlite(connection)
            .Options;

        await using var context = new OrdersDbContext(options);
        await context.Database.EnsureCreatedAsync(TestContext.Current.CancellationToken);

        var firstOrder = new Order(Guid.NewGuid());
        await Task.Delay(10, TestContext.Current.CancellationToken);
        var newestOrder = new Order(Guid.NewGuid());

        context.Orders.AddRange(firstOrder, newestOrder);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var orders = await new OrderRepository(context)
            .GetAllAsync(TestContext.Current.CancellationToken);

        Assert.Equal([newestOrder.Id, firstOrder.Id], orders.Select(order => order.Id));
    }
}
