using Microsoft.AspNetCore.Mvc;
using SystemDistributedOrders.Application.Orders.Commands.AddOrderItem;
using SystemDistributedOrders.Application.Orders.Commands.CancelOrder;
using SystemDistributedOrders.Application.Orders.Commands.CreateOrder;
using SystemDistributedOrders.Application.Orders.Commands.MarkOrderAsPaid;
using SystemDistributedOrders.Application.Orders.Commands.SubmitOrder;
using SystemDistributedOrders.Application.Orders.DTOs;
using SystemDistributedOrders.Application.Orders.Queries.GetOrderById;
using SystemDistributedOrders.Application.Orders.Queries.GetOrders;

namespace SystemDistributedOrders.Api.Controllers;

[ApiController]
[Route("api/orders")]
public sealed class OrdersController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<IReadOnlyCollection<OrderResponse>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<OrderResponse>>> GetAll(
        [FromServices] GetOrdersQueryHandler handler,
        CancellationToken cancellationToken)
    {
        var orders = await handler.HandleAsync(new GetOrdersQuery(), cancellationToken);
        return Ok(orders);
    }

    [HttpGet("{orderId:guid}")]
    [ProducesResponseType<OrderResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrderResponse>> GetById(
        Guid orderId,
        [FromServices] GetOrderByIdQueryHandler handler,
        CancellationToken cancellationToken)
    {
        var order = await handler.HandleAsync(new GetOrderByIdQuery(orderId), cancellationToken);
        return Ok(order);
    }

    [HttpPost]
    [ProducesResponseType<CreateOrderResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CreateOrderResponse>> Create(
        CreateOrderRequest request,
        [FromServices] CreateOrderCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var orderId = await handler.HandleAsync(
            new CreateOrderCommand(request.CustomerId),
            cancellationToken);

        return CreatedAtAction(nameof(GetById), new { orderId }, new CreateOrderResponse(orderId));
    }

    [HttpPost("{orderId:guid}/items")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> AddItem(
        Guid orderId,
        AddOrderItemRequest request,
        [FromServices] AddOrderItemCommandHandler handler,
        CancellationToken cancellationToken)
    {
        await handler.HandleAsync(
            new AddOrderItemCommand(orderId, request.ProductId, request.Quantity),
            cancellationToken);

        return NoContent();
    }

    [HttpPost("{orderId:guid}/submit")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Submit(
        Guid orderId,
        [FromServices] SubmitOrderCommandHandler handler,
        CancellationToken cancellationToken)
    {
        await handler.HandleAsync(new SubmitOrderCommand(orderId), cancellationToken);
        return NoContent();
    }

    [HttpPost("{orderId:guid}/pay")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> MarkAsPaid(
        Guid orderId,
        [FromServices] MarkOrderAsPaidCommandHandler handler,
        CancellationToken cancellationToken)
    {
        await handler.HandleAsync(new MarkOrderAsPaidCommand(orderId), cancellationToken);
        return NoContent();
    }

    [HttpPost("{orderId:guid}/cancel")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Cancel(
        Guid orderId,
        [FromServices] CancelOrderCommandHandler handler,
        CancellationToken cancellationToken)
    {
        await handler.HandleAsync(new CancelOrderCommand(orderId), cancellationToken);
        return NoContent();
    }
}

public sealed record CreateOrderRequest(Guid CustomerId);
public sealed record CreateOrderResponse(Guid OrderId);
public sealed record AddOrderItemRequest(Guid ProductId, int Quantity);
