using SystemDistributedOrders.Application.Abstractions.Messaging;
using SystemDistributedOrders.Application.Abstractions.Persistence;
using SystemDistributedOrders.Application.Orders.Commands.SubmitOrder;
using SystemDistributedOrders.Contracts.Events;
using SystemDistributedOrders.Domain.Entities;

namespace SystemDistributedOrders.Application.Tests.Orders.Commands;

public sealed class SubmitOrderCommandHandlerTests
{
    [Fact]
    public async Task HandleAsync_ShouldSaveOrderAndPublishOrderSubmitted()
    {
        var order = new Order(Guid.NewGuid());
        order.AddItem(Guid.NewGuid(), "Produto", 25m, 2);

        var repository = new StubOrderRepository(order);
        var unitOfWork = new SpyUnitOfWork();
        var publisher = new SpyOrderSubmittedPublisher();
        var handler = new SubmitOrderCommandHandler(repository, unitOfWork, publisher);

        await handler.HandleAsync(
            new SubmitOrderCommand(order.Id),
            TestContext.Current.CancellationToken);

        Assert.Equal(1, unitOfWork.SaveChangesCalls);

        var publishedEvent = Assert.Single(publisher.Messages);
        Assert.NotEqual(Guid.Empty, publishedEvent.EventId);
        Assert.Equal(order.Id, publishedEvent.OrderId);
        Assert.Equal(order.CustomerId, publishedEvent.CustomerId);
        Assert.Equal(50m, publishedEvent.Total);
        Assert.Equal(1, publishedEvent.Version);
    }

    private sealed class StubOrderRepository(Order order) : IOrderRepository
    {
        public Task AddAsync(Order newOrder, CancellationToken cancellationToken = default) =>
            Task.CompletedTask;

        public Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
            Task.FromResult<Order?>(id == order.Id ? order : null);

        public Task<IReadOnlyCollection<Order>> GetAllAsync(
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyCollection<Order>>([order]);
    }

    private sealed class SpyUnitOfWork : IUnitOfWork
    {
        public int SaveChangesCalls { get; private set; }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            SaveChangesCalls++;
            return Task.FromResult(1);
        }
    }

    private sealed class SpyOrderSubmittedPublisher : IOrderSubmittedPublisher
    {
        public List<OrderSubmittedEvent> Messages { get; } = [];

        public Task PublishAsync(
            OrderSubmittedEvent message,
            CancellationToken cancellationToken = default)
        {
            Messages.Add(message);
            return Task.CompletedTask;
        }
    }
}
