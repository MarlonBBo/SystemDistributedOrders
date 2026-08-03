using SystemDistributedOrders.Application.Abstractions.Persistence;
using SystemDistributedOrders.Application.Common.Exceptions;

namespace SystemDistributedOrders.Application.Orders.Commands.CancelOrder;

public sealed class CancelOrderCommandHandler
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CancelOrderCommandHandler(
        IOrderRepository orderRepository,
        IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task HandleAsync(
        CancelOrderCommand command,
        CancellationToken cancellationToken = default)
    {
        if (command is null)
            throw new ValidationException(nameof(command), "O comando é obrigatório.");

        if (command.OrderId == Guid.Empty)
            throw new ValidationException(nameof(command.OrderId), "O pedido é obrigatório.");

        var order = await _orderRepository.GetByIdAsync(command.OrderId, cancellationToken)
            ?? throw new NotFoundException("Pedido", command.OrderId);

        order.Cancel();

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
