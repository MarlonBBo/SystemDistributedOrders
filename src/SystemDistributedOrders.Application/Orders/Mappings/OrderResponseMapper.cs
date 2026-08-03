using SystemDistributedOrders.Application.Orders.DTOs;
using SystemDistributedOrders.Domain.Entities;

namespace SystemDistributedOrders.Application.Orders.Mappings;

internal static class OrderResponseMapper
{
    public static OrderResponse ToResponse(this Order order)
    {
        var items = order.Items
            .Select(item => new OrderItemResponse(
                item.Id,
                item.ProductId,
                item.ProductName,
                item.Price,
                item.Quantity,
                item.Total))
            .ToArray();

        return new OrderResponse(
            order.Id,
            order.CustomerId,
            order.Status,
            items,
            order.Total,
            order.CreatedAt,
            order.UpdatedAt);
    }
}
