using Microsoft.OpenApi;
using SystemDistributedOrders.Application.Orders.Commands.AddOrderItem;
using SystemDistributedOrders.Application.Orders.Commands.CancelOrder;
using SystemDistributedOrders.Application.Orders.Commands.CreateOrder;
using SystemDistributedOrders.Application.Orders.Commands.MarkOrderAsPaid;
using SystemDistributedOrders.Application.Orders.Commands.SubmitOrder;
using SystemDistributedOrders.Application.Orders.Queries.GetOrderById;
using SystemDistributedOrders.Application.Orders.Queries.GetOrders;
using SystemDistributedOrders.Application.Products.Queries.GetProductById;
using SystemDistributedOrders.Application.Products.Queries.GetProducts;
using SystemDistributedOrders.Api.Infrastructure;

namespace SystemDistributedOrders.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        services.AddExceptionHandler<ApiExceptionHandler>();
        services.AddProblemDetails();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "System Distributed Orders API",
                Version = "v1",
                Description = "API para gerenciamento de pedidos e publicação de eventos no Kafka."
            });
        });

        services.AddScoped<CreateOrderCommandHandler>();
        services.AddScoped<AddOrderItemCommandHandler>();
        services.AddScoped<SubmitOrderCommandHandler>();
        services.AddScoped<MarkOrderAsPaidCommandHandler>();
        services.AddScoped<CancelOrderCommandHandler>();
        services.AddScoped<GetOrderByIdQueryHandler>();
        services.AddScoped<GetOrdersQueryHandler>();
        services.AddScoped<GetProductByIdQueryHandler>();
        services.AddScoped<GetProductsQueryHandler>();

        return services;
    }
}
