
namespace SystemDistributedOrders.Contracts.Events;
    public sealed record OrderSubmittedEvent
    (
        Guid EventId,
        Guid OrderId,
        Guid CustomerId,
        decimal Total,
        DateTimeOffset SubmittedAtUtc,
        int Version = 1
    );
