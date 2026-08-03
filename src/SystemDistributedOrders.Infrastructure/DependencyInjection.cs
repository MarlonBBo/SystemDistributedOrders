using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SystemDistributedOrders.Application.Abstractions.Messaging;
using SystemDistributedOrders.Application.Abstractions.Persistence;
using SystemDistributedOrders.Infrastructure.Messaging.Kafka;
using SystemDistributedOrders.Infrastructure.Persistence;
using SystemDistributedOrders.Infrastructure.Persistence.Repositories;

namespace SystemDistributedOrders.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("SqlServer")
            ?? throw new InvalidOperationException(
                "A connection string 'SqlServer' não foi configurada.");

        services.AddDbContext<OrdersDbContext>(options =>
            options.UseSqlServer(connectionString, sqlServerOptions =>
                sqlServerOptions.EnableRetryOnFailure()));

        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IProductCatalog, ProductCatalog>();
        services.AddScoped<IUnitOfWork>(provider =>
            provider.GetRequiredService<OrdersDbContext>());

        services.AddOptions<KafkaOptions>()
            .Bind(configuration.GetRequiredSection(KafkaOptions.SectionName))
            .Validate(
                options => !options.Enabled || !string.IsNullOrWhiteSpace(options.BootstrapServers),
                "Kafka:BootstrapServers é obrigatório.")
            .Validate(
                options => !options.Enabled || !string.IsNullOrWhiteSpace(options.OrderSubmittedTopic),
                "Kafka:OrderSubmittedTopic é obrigatório.")
            .Validate(
                options => !options.Enabled || !string.IsNullOrWhiteSpace(options.OrderSubmittedConsumerGroup),
                "Kafka:OrderSubmittedConsumerGroup é obrigatório.")
            .ValidateOnStart();

        services.AddSingleton<IOrderSubmittedPublisher, KafkaEventPublisher>();
        services.AddScoped<IOrderSubmittedEventHandler, OrderSubmittedEventHandler>();
        services.AddHostedService<OrderSubmittedConsumer>();

        return services;
    }
}
