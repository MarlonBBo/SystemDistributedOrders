using SystemDistributedOrders.Domain.Entities;

namespace SystemDistributedOrders.Application.Abstractions.Persistence;

public interface IProductCatalog
{
    Task<IReadOnlyCollection<Product>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<Product?> GetByIdAsync(
        Guid productId,
        CancellationToken cancellationToken = default);
}
