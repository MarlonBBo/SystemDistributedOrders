namespace SystemDistributedOrders.Infrastructure.Messaging.Kafka;

public sealed class KafkaOptions
{
    public const string SectionName = "Kafka";

    public bool Enabled { get; init; } = true;
    public required string BootstrapServers { get; init; }
    public string OrderSubmittedTopic { get; init; } = "orders.submitted.v1";
    public string OrderSubmittedConsumerGroup { get; init; } = "system-distributed-orders";
}
