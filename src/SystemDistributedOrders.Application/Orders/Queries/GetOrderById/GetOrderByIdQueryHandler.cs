using SystemDistributedOrders.Application.Abstractions.Persistence;
using SystemDistributedOrders.Application.Common.Exceptions;
using SystemDistributedOrders.Application.Orders.DTOs;
using SystemDistributedOrders.Application.Orders.Mappings;

namespace SystemDistributedOrders.Application.Orders.Queries.GetOrderById;

public sealed class GetOrderByIdQueryHandler
{
    private readonly IOrderRepository _orderRepository;

    public GetOrderByIdQueryHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<OrderResponse> HandleAsync(
        GetOrderByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        if (query is null)
            throw new ValidationException(nameof(query), "A consulta é obrigatória.");

        if (query.OrderId == Guid.Empty)
            throw new ValidationException(nameof(query.OrderId), "O pedido é obrigatório.");

        var order = await _orderRepository.GetByIdAsync(query.OrderId, cancellationToken)
            ?? throw new NotFoundException("Pedido", query.OrderId);

        return order.ToResponse();
    }
}
