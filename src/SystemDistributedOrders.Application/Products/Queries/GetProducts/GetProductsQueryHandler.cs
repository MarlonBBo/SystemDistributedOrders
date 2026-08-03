using SystemDistributedOrders.Application.Abstractions.Persistence;
using SystemDistributedOrders.Application.Products.DTOs;

namespace SystemDistributedOrders.Application.Products.Queries.GetProducts;

public sealed class GetProductsQueryHandler
{
    private readonly IProductCatalog _productCatalog;

    public GetProductsQueryHandler(IProductCatalog productCatalog)
    {
        _productCatalog = productCatalog;
    }

    public async Task<IReadOnlyCollection<ProductResponse>> HandleAsync(
        GetProductsQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        var products = await _productCatalog.GetAllAsync(cancellationToken);

        return products
            .Select(product => new ProductResponse(
                product.Id,
                product.Name,
                product.Price,
                product.CreatedAt,
                product.UpdatedAt))
            .ToArray();
    }
}
