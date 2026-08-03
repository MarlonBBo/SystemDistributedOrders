using SystemDistributedOrders.Application.Abstractions.Persistence;
using SystemDistributedOrders.Application.Common.Exceptions;
using SystemDistributedOrders.Application.Products.DTOs;

namespace SystemDistributedOrders.Application.Products.Queries.GetProductById;

public sealed class GetProductByIdQueryHandler
{
    private readonly IProductCatalog _productCatalog;

    public GetProductByIdQueryHandler(IProductCatalog productCatalog)
    {
        _productCatalog = productCatalog;
    }

    public async Task<ProductResponse> HandleAsync(
        GetProductByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        if (query is null)
            throw new ValidationException(nameof(query), "A consulta é obrigatória.");

        if (query.ProductId == Guid.Empty)
            throw new ValidationException(nameof(query.ProductId), "O produto é obrigatório.");

        var product = await _productCatalog.GetByIdAsync(query.ProductId, cancellationToken)
            ?? throw new NotFoundException("Produto", query.ProductId);

        return new ProductResponse(
            product.Id,
            product.Name,
            product.Price,
            product.CreatedAt,
            product.UpdatedAt);
    }
}
