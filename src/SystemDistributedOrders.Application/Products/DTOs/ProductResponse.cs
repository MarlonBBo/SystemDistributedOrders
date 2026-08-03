namespace SystemDistributedOrders.Application.Products.DTOs;

public sealed record ProductResponse(
    Guid Id,
    string Name,
    decimal Price,
    DateTime CreatedAt,
    DateTime UpdatedAt);
