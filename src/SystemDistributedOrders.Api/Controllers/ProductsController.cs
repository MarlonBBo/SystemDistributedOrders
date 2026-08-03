using Microsoft.AspNetCore.Mvc;
using SystemDistributedOrders.Application.Products.DTOs;
using SystemDistributedOrders.Application.Products.Queries.GetProductById;
using SystemDistributedOrders.Application.Products.Queries.GetProducts;

namespace SystemDistributedOrders.Api.Controllers;

[ApiController]
[Route("api/products")]
public sealed class ProductsController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<IReadOnlyCollection<ProductResponse>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<ProductResponse>>> GetAll(
        [FromServices] GetProductsQueryHandler handler,
        CancellationToken cancellationToken)
    {
        var products = await handler.HandleAsync(new GetProductsQuery(), cancellationToken);
        return Ok(products);
    }

    [HttpGet("{productId:guid}")]
    [ProducesResponseType<ProductResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductResponse>> GetById(
        Guid productId,
        [FromServices] GetProductByIdQueryHandler handler,
        CancellationToken cancellationToken)
    {
        var product = await handler.HandleAsync(
            new GetProductByIdQuery(productId),
            cancellationToken);

        return Ok(product);
    }
}
