using SystemDistributedOrders.Contracts.Events;

namespace SystemDistributedOrders.Application.Abstractions.Messaging;

public interface IOrderSubmittedPublisher
{
    Task PublishAsync(
        OrderSubmittedEvent message,
        CancellationToken cancellationToken = default);
}
