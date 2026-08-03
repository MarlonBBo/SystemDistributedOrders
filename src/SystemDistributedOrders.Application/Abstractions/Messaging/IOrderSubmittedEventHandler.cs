using SystemDistributedOrders.Contracts.Events;

namespace SystemDistributedOrders.Application.Abstractions.Messaging;

public interface IOrderSubmittedEventHandler
{
    Task HandleAsync(
        OrderSubmittedEvent message,
        CancellationToken cancellationToken = default);
}
