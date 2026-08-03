using System.Net;
using System.Net.Http.Json;
using SystemDistributedOrders.Api.Controllers;
using SystemDistributedOrders.Application.Orders.DTOs;
using SystemDistributedOrders.Application.Products.DTOs;
using SystemDistributedOrders.Domain.Enum;

namespace SystemDistributedOrders.Api.Tests;

public sealed class OrdersApiTests
{
    [Fact]
    public async Task OrderFlow_ShouldCreateAddItemSubmitAndPay()
    {
        await using var factory = new CustomWebApplicationFactory();
        using var client = await factory.CreateInitializedClientAsync(
            TestContext.Current.CancellationToken);

        var products = await client.GetFromJsonAsync<ProductResponse[]>(
            "/api/products",
            TestContext.Current.CancellationToken);
        var product = Assert.Single(products!);

        var createResponse = await client.PostAsJsonAsync(
            "/api/orders",
            new CreateOrderRequest(Guid.NewGuid()),
            TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var createdOrder = await createResponse.Content.ReadFromJsonAsync<CreateOrderResponse>(
            TestContext.Current.CancellationToken);
        Assert.NotNull(createdOrder);

        var addItemResponse = await client.PostAsJsonAsync(
            $"/api/orders/{createdOrder.OrderId}/items",
            new AddOrderItemRequest(product.Id, 2),
            TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.NoContent, addItemResponse.StatusCode);

        var submitResponse = await client.PostAsync(
            $"/api/orders/{createdOrder.OrderId}/submit",
            null,
            TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.NoContent, submitResponse.StatusCode);

        var payResponse = await client.PostAsync(
            $"/api/orders/{createdOrder.OrderId}/pay",
            null,
            TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.NoContent, payResponse.StatusCode);

        var order = await client.GetFromJsonAsync<OrderResponse>(
            $"/api/orders/{createdOrder.OrderId}",
            TestContext.Current.CancellationToken);

        Assert.NotNull(order);
        Assert.Equal(OrderStatus.Paid, order.Status);
        Assert.Single(order.Items);
        Assert.Equal(99.80m, order.Total);
    }

    [Fact]
    public async Task GetUnknownOrder_ShouldReturnNotFoundProblemDetails()
    {
        await using var factory = new CustomWebApplicationFactory();
        using var client = await factory.CreateInitializedClientAsync(
            TestContext.Current.CancellationToken);

        var response = await client.GetAsync(
            $"/api/orders/{Guid.NewGuid()}",
            TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }
}
