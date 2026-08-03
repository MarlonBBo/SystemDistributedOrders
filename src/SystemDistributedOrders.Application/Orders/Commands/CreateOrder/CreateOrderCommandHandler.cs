using SystemDistributedOrders.Application.Abstractions.Persistence;
using SystemDistributedOrders.Application.Common.Exceptions;
using SystemDistributedOrders.Domain.Entities;

namespace SystemDistributedOrders.Application.Orders.Commands.CreateOrder;

public sealed class CreateOrderCommandHandler
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateOrderCommandHandler(
        IOrderRepository orderRepository,
        IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> HandleAsync(
        CreateOrderCommand command,
        CancellationToken cancellationToken = default)
    {
        if (command is null)
            throw new ValidationException(nameof(command), "O comando é obrigatório.");

        if (command.CustomerId == Guid.Empty)
            throw new ValidationException(nameof(command.CustomerId), "O cliente é obrigatório.");

        var order = new Order(command.CustomerId);

        await _orderRepository.AddAsync(order, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return order.Id;
    }
}
