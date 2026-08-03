
using SystemDistributedOrders.Domain.Entities;
using SystemDistributedOrders.Domain.Enum;

namespace SystemDistributedOrders.Domain.Tests.Entities
{
    public sealed class OrderTests
    {
        [Fact]
        public void Constructor_WithValidCustomer_ShouldCreateOrderDraft()
        {
            //Arrange
            var customerId = Guid.NewGuid();

            //Act
            var order = new Order(customerId);

            //Assert
            Assert.NotEqual(customerId, order.Id);
            Assert.Equal(customerId, order.CustomerId);
            Assert.Empty(order.Items);
            Assert.Equal(0, order.Total);
            Assert.Equal(OrderStatus.Draft, order.Status);
        }

        [Fact]
        public void Contructor_WithEmptyCustomerId_ShouldThrowArgumentException()
        {
            var exception = Assert.Throws<ArgumentException>(() => new Order(Guid.Empty));

            Assert.Equal("O cliente é obrigatório.", exception.Message);
        }

        [Fact]
        public void AddItem_WithValidData_ShouldAddItemAndUpdateTotal()
        {
            //Arrange
            var customerId = Guid.NewGuid();

            var productId = Guid.NewGuid();
            var productName = "Coca-cola";
            decimal price = 10.60m;
            var quantity = 2;

            //Act
            var order = new Order(customerId);

            order.AddItem(productId, productName, price, quantity);

            //Assert
            var item = Assert.Single(order.Items);
            Assert.Equal(order.Id, item.OrderId);
            Assert.Equal(productId, item.ProductId);
            Assert.Equal("Coca-cola", item.ProductName);
            Assert.Equal(10.60m, item.Price);
            Assert.Equal(2, item.Quantity);
            Assert.Equal(21.20m, item.Total);
            Assert.Equal(21.20m, order.Total);
            Assert.NotEqual(default, order.UpdatedAt);
        }

        [Fact]
        public void Submit_WithOrderDraft_ShouldChangeOrderStatusToAwaitingPayment()
        {
            //Arrange
            var order = CreateOrderWithItem();

            //Act
            order.Submit();

            //Assert
            Assert.Equal(OrderStatus.AwaitingPayment, order.Status);
        }
        
        private static Order CreateOrderWithItem()
        {
            var customerId = Guid.NewGuid();

            var productId = Guid.NewGuid();
            var productName = "Coca-cola";
            decimal price = 10.60m;
            var quantity = 2;

            var order = new Order(customerId);

            order.AddItem(productId, productName, price, quantity);

            return order;
        }
    }
}
