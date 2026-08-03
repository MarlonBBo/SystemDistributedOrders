using SystemDistributedOrders.Application.Abstractions.Persistence;
using SystemDistributedOrders.Application.Common.Exceptions;
using SystemDistributedOrders.Application.Products.Queries.GetProductById;
using SystemDistributedOrders.Domain.Entities;

namespace SystemDistributedOrders.Application.Tests.Products.Queries;

public sealed class GetProductByIdQueryHandlerTests
{
    [Fact]
    public async Task HandleAsync_WithExistingProduct_ShouldReturnDto()
    {
        var product = new Product("Café", 12.50m);
        var handler = new GetProductByIdQueryHandler(new ProductCatalogStub(product));

        var response = await handler.HandleAsync(
            new GetProductByIdQuery(product.Id),
            TestContext.Current.CancellationToken);

        Assert.Equal(product.Id, response.Id);
        Assert.Equal("Café", response.Name);
        Assert.Equal(12.50m, response.Price);
        Assert.Equal(product.CreatedAt, response.CreatedAt);
        Assert.Equal(product.UpdatedAt, response.UpdatedAt);
    }

    [Fact]
    public async Task HandleAsync_WithEmptyProductId_ShouldThrowValidationException()
    {
        var handler = new GetProductByIdQueryHandler(new ProductCatalogStub(null));

        var exception = await Assert.ThrowsAsync<ValidationException>(
            () => handler.HandleAsync(
                new GetProductByIdQuery(Guid.Empty),
                TestContext.Current.CancellationToken));

        Assert.Contains(nameof(GetProductByIdQuery.ProductId), exception.Errors.Keys);
    }

    [Fact]
    public async Task HandleAsync_WithMissingProduct_ShouldThrowNotFoundException()
    {
        var productId = Guid.NewGuid();
        var handler = new GetProductByIdQueryHandler(new ProductCatalogStub(null));

        var exception = await Assert.ThrowsAsync<NotFoundException>(
            () => handler.HandleAsync(
                new GetProductByIdQuery(productId),
                TestContext.Current.CancellationToken));

        Assert.Equal("Produto", exception.ResourceName);
        Assert.Equal(productId, exception.Key);
    }

    private sealed class ProductCatalogStub(Product? product) : IProductCatalog
    {
        public Task<IReadOnlyCollection<Product>> GetAllAsync(
            CancellationToken cancellationToken = default)
        {
            IReadOnlyCollection<Product> products = product is null ? [] : [product];
            return Task.FromResult(products);
        }

        public Task<Product?> GetByIdAsync(
            Guid productId,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(product);
        }
    }
}
