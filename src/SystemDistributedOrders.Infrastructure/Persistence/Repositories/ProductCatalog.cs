using Microsoft.EntityFrameworkCore;
using SystemDistributedOrders.Application.Abstractions.Persistence;
using SystemDistributedOrders.Domain.Entities;

namespace SystemDistributedOrders.Infrastructure.Persistence.Repositories;

internal sealed class ProductCatalog : IProductCatalog
{
    private readonly OrdersDbContext _context;

    public ProductCatalog(OrdersDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyCollection<Product>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .AsNoTracking()
            .OrderBy(product => product.Name)
            .ToArrayAsync(cancellationToken);
    }

    public Task<Product?> GetByIdAsync(
        Guid productId,
        CancellationToken cancellationToken = default)
    {
        return _context.Products
            .AsNoTracking()
            .SingleOrDefaultAsync(product => product.Id == productId, cancellationToken);
    }
}
