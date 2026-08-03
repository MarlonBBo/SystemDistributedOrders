using SystemDistributedOrders.Domain.Enum;

namespace SystemDistributedOrders.Application.Orders.DTOs;

public sealed record OrderResponse(
    Guid Id,
    Guid CustomerId,
    OrderStatus Status,
    IReadOnlyCollection<OrderItemResponse> Items,
    decimal Total,
    DateTime CreatedAt,
    DateTime UpdatedAt);
