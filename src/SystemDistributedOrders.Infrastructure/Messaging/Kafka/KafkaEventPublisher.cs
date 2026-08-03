using System.Text;
using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Options;
using SystemDistributedOrders.Application.Abstractions.Messaging;
using SystemDistributedOrders.Contracts.Events;

namespace SystemDistributedOrders.Infrastructure.Messaging.Kafka;

internal sealed class KafkaEventPublisher : IOrderSubmittedPublisher, IDisposable
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);
    private readonly IProducer<string, string> _producer;
    private readonly KafkaOptions _options;

    public KafkaEventPublisher(IOptions<KafkaOptions> options)
    {
        _options = options.Value;

        var config = new ProducerConfig
        {
            BootstrapServers = _options.BootstrapServers,
            ClientId = "system-distributed-orders-api",
            Acks = Acks.All,
            EnableIdempotence = true
        };

        _producer = new ProducerBuilder<string, string>(config).Build();
    }

    public async Task PublishAsync(
        OrderSubmittedEvent message,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(message);

        var kafkaMessage = new Message<string, string>
        {
            Key = message.OrderId.ToString(),
            Value = JsonSerializer.Serialize(message, SerializerOptions),
            Headers = new Headers
            {
                { "event-type", Encoding.UTF8.GetBytes("OrderSubmitted") },
                { "event-version", Encoding.UTF8.GetBytes(message.Version.ToString()) },
                { "content-type", Encoding.UTF8.GetBytes("application/json") }
            }
        };

        await _producer.ProduceAsync(
            _options.OrderSubmittedTopic,
            kafkaMessage,
            cancellationToken);
    }

    public void Dispose()
    {
        _producer.Flush(TimeSpan.FromSeconds(5));
        _producer.Dispose();
    }
}
