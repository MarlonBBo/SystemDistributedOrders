using SystemDistributedOrders.Application.Abstractions.Persistence;
using SystemDistributedOrders.Application.Common.Exceptions;
using SystemDistributedOrders.Application.Orders.DTOs;
using SystemDistributedOrders.Application.Orders.Mappings;

namespace SystemDistributedOrders.Application.Orders.Queries.GetOrders;

public sealed class GetOrdersQueryHandler
{
    private readonly IOrderRepository _orderRepository;

    public GetOrdersQueryHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<IReadOnlyCollection<OrderResponse>> HandleAsync(
        GetOrdersQuery query,
        CancellationToken cancellationToken = default)
    {
        if (query is null)
            throw new ValidationException(nameof(query), "A consulta é obrigatória.");

        var orders = await _orderRepository.GetAllAsync(cancellationToken);

        return orders
            .Select(order => order.ToResponse())
            .ToArray();
    }
}
