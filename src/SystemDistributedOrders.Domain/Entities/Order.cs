
using SystemDistributedOrders.Domain.Commons;
using SystemDistributedOrders.Domain.Enum;

namespace SystemDistributedOrders.Domain.Entities
{
    public class Order : BaseEntity
    {
        private readonly List<OrderItem> _item = [];

        public Guid CustomerId { get; private set; }
        public OrderStatus Status { get; private set; }
        public IReadOnlyCollection<OrderItem> Items => _item.AsReadOnly();
        public decimal Total => _item.Sum(item => item.Total);

        private Order() { }

        public Order(Guid customerId)
        {
            if (customerId == Guid.Empty)
                throw new ArgumentException("O cliente é obrigatório.");

            CustomerId = customerId;
            Status = OrderStatus.Draft;
        }

        public void AddItem(Guid productId, string productName, decimal price, int quantity)
        {
            EnsureIsDraft();

            var existingItem = _item.FirstOrDefault(item => item.ProductId == productId);

            if (existingItem is not null)
            {
                existingItem.IncreaseQuantity(quantity);
                MarkAsUpdate();
                return;
            }

            var item = new OrderItem(Id, productId, productName, price, quantity);

            _item.Add(item);
            MarkAsUpdate();
        }

        public void Submit()
        {
            EnsureIsDraft();

            if (_item.Count == 0)
                throw new ArgumentException("É obrigatório pelo menos 1 item para fechar um pedido.");

            Status = OrderStatus.AwaitingPayment;
            MarkAsUpdate();
        }

        public void MarkAsPaid()
        {
            if (Status != OrderStatus.AwaitingPayment)
                throw new ArgumentException("O pedido não está aguardando pagamento.");

            Status = OrderStatus.Paid;
            MarkAsUpdate();
        }

        public void Cancel()
        {
            if (Status is OrderStatus.Paid or OrderStatus.Delivered or OrderStatus.Cancelled)
                throw new InvalidOperationException("O pedido não pode ser cancelado.");

            Status = OrderStatus.Cancelled;
            MarkAsUpdate();
        }

        private void EnsureIsDraft()
        {
            if (Status != OrderStatus.Draft)
                throw new ArgumentException("Somente pedidos em rascunhos podem ser alterados");
        }
    }
}
