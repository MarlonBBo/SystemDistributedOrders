using Microsoft.Extensions.Logging;
using SystemDistributedOrders.Application.Abstractions.Messaging;
using SystemDistributedOrders.Contracts.Events;

namespace SystemDistributedOrders.Infrastructure.Messaging.Kafka;

internal sealed class OrderSubmittedEventHandler(
    ILogger<OrderSubmittedEventHandler> logger) : IOrderSubmittedEventHandler
{
    public Task HandleAsync(
        OrderSubmittedEvent message,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        logger.LogInformation(
            "OrderSubmitted processado. EventId: {EventId}, OrderId: {OrderId}, CustomerId: {CustomerId}, Total: {Total}",
            message.EventId,
            message.OrderId,
            message.CustomerId,
            message.Total);

        return Task.CompletedTask;
    }
}
