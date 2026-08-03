using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using SystemDistributedOrders.Application.Abstractions.Messaging;
using SystemDistributedOrders.Contracts.Events;
using SystemDistributedOrders.Domain.Entities;
using SystemDistributedOrders.Infrastructure.Persistence;

namespace SystemDistributedOrders.Api.Tests;

public sealed class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly SqliteConnection _connection = new("Data Source=:memory:");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        _connection.Open();
        builder.UseEnvironment("Testing");
        builder.UseSetting("Kafka:Enabled", "false");
        builder.ConfigureLogging(logging => logging.ClearProviders());

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<OrdersDbContext>>();
            services.RemoveAll<IDbContextOptionsConfiguration<OrdersDbContext>>();
            services.RemoveAll<OrdersDbContext>();
            services.AddDbContext<OrdersDbContext>(options => options.UseSqlite(_connection));

            services.RemoveAll<IOrderSubmittedPublisher>();
            services.AddSingleton<IOrderSubmittedPublisher, TestOrderSubmittedPublisher>();
        });
    }

    public async Task<HttpClient> CreateInitializedClientAsync(
        CancellationToken cancellationToken = default)
    {
        var client = CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        await using var scope = Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<OrdersDbContext>();
        await context.Database.EnsureCreatedAsync(cancellationToken);
        context.Products.Add(new Product("Produto da API", 49.90m));
        await context.SaveChangesAsync(cancellationToken);

        return client;
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing)
            _connection.Dispose();
    }

    private sealed class TestOrderSubmittedPublisher : IOrderSubmittedPublisher
    {
        public Task PublishAsync(
            OrderSubmittedEvent message,
            CancellationToken cancellationToken = default) => Task.CompletedTask;
    }
}
