using SystemDistributedOrders.Domain.Entities;

namespace SystemDistributedOrders.Application.Abstractions.Persistence;

public interface IOrderRepository
{
    Task AddAsync(Order order, CancellationToken cancellationToken = default);
    Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Order>> GetAllAsync(CancellationToken cancellationToken = default);
}
