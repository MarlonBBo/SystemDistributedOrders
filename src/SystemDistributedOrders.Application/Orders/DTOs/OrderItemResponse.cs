namespace SystemDistributedOrders.Application.Orders.DTOs;

public sealed record OrderItemResponse(
    Guid Id,
    Guid ProductId,
    string ProductName,
    decimal Price,
    int Quantity,
    decimal Total);
