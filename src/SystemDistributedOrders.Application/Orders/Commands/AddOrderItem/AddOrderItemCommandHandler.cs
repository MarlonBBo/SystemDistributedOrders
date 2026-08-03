using SystemDistributedOrders.Application.Abstractions.Persistence;
using SystemDistributedOrders.Application.Common.Exceptions;

namespace SystemDistributedOrders.Application.Orders.Commands.AddOrderItem;

public sealed class AddOrderItemCommandHandler
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductCatalog _productCatalog;
    private readonly IUnitOfWork _unitOfWork;

    public AddOrderItemCommandHandler(
        IOrderRepository orderRepository,
        IProductCatalog productCatalog,
        IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _productCatalog = productCatalog;
        _unitOfWork = unitOfWork;
    }

    public async Task HandleAsync(
        AddOrderItemCommand command,
        CancellationToken cancellationToken = default)
    {
        if (command is null)
            throw new ValidationException(nameof(command), "O comando é obrigatório.");

        if (command.OrderId == Guid.Empty)
            throw new ValidationException(nameof(command.OrderId), "O pedido é obrigatório.");

        if (command.ProductId == Guid.Empty)
            throw new ValidationException(nameof(command.ProductId), "O produto é obrigatório.");

        if (command.Quantity <= 0)
            throw new ValidationException(nameof(command.Quantity), "A quantidade deve ser maior que zero.");

        var order = await _orderRepository.GetByIdAsync(command.OrderId, cancellationToken)
            ?? throw new NotFoundException("Pedido", command.OrderId);

        var product = await _productCatalog.GetByIdAsync(command.ProductId, cancellationToken)
            ?? throw new NotFoundException("Produto", command.ProductId);

        order.AddItem(product.Id, product.Name, product.Price, command.Quantity);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
