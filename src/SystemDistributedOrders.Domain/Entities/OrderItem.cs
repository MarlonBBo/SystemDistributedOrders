using SystemDistributedOrders.Domain.Commons;

namespace SystemDistributedOrders.Domain.Entities
{
    public class OrderItem : BaseEntity
    {
        public Guid OrderId { get; private set; }
        public Guid ProductId { get; private set; }
        public string ProductName { get; private set; }
        public decimal Price { get; private set; }
        public int Quantity { get; private set; }
        public decimal Total => Price * Quantity;

        private OrderItem()
        {
            ProductName = string.Empty;
        }

        internal OrderItem(Guid orderId, Guid productId, string productName, decimal price, int quantity)
        {
            if (orderId == Guid.Empty)
                throw new ArgumentException("O pedido é obrigatório");
            if (productId == Guid.Empty)
                throw new ArgumentException("O produto é obrigatório");
            if (string.IsNullOrWhiteSpace(productName))
                throw new ArgumentException("O nome do produto é obrigatório");
            if (price <= 0)
                throw new ArgumentException("O preço deve ser maior que zero.");
            if (quantity <= 0)
                throw new ArgumentException("A quantidade deve ser maior que zero.");

            OrderId = orderId;
            ProductId = productId;
            ProductName = productName;
            Price = price;
            Quantity = quantity;
        }

        internal void IncreaseQuantity(int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("A quantidade deve ser maior que zero.");

            Quantity += quantity;
            MarkAsUpdate();
        }
    }
}
