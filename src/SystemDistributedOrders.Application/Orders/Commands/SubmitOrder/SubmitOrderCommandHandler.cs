using SystemDistributedOrders.Application.Abstractions.Messaging;
using SystemDistributedOrders.Application.Abstractions.Persistence;
using SystemDistributedOrders.Application.Common.Exceptions;
using SystemDistributedOrders.Contracts.Events;

namespace SystemDistributedOrders.Application.Orders.Commands.SubmitOrder;

public sealed class SubmitOrderCommandHandler
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOrderSubmittedPublisher _publisher;

    public SubmitOrderCommandHandler(
        IOrderRepository orderRepository,
        IUnitOfWork unitOfWork,
        IOrderSubmittedPublisher publisher)
    {
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
        _publisher = publisher;
    }

    public async Task HandleAsync(
        SubmitOrderCommand command,
        CancellationToken cancellationToken = default)
    {
        if (command is null)
            throw new ValidationException(nameof(command), "O comando é obrigatório.");

        if (command.OrderId == Guid.Empty)
            throw new ValidationException(nameof(command.OrderId), "O pedido é obrigatório.");

        var order = await _orderRepository.GetByIdAsync(command.OrderId, cancellationToken)
            ?? throw new NotFoundException("Pedido", command.OrderId);

        order.Submit();

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _publisher.PublishAsync(
            new OrderSubmittedEvent(
                Guid.NewGuid(),
                order.Id,
                order.CustomerId,
                order.Total,
                DateTimeOffset.UtcNow),
            cancellationToken);
    }
}
