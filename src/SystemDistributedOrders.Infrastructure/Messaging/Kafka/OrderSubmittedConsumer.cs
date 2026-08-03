using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SystemDistributedOrders.Application.Abstractions.Messaging;
using SystemDistributedOrders.Contracts.Events;

namespace SystemDistributedOrders.Infrastructure.Messaging.Kafka;

internal sealed class OrderSubmittedConsumer(
    IOptions<KafkaOptions> options,
    IServiceScopeFactory scopeFactory,
    ILogger<OrderSubmittedConsumer> logger) : BackgroundService
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);
    private readonly KafkaOptions _options = options.Value;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_options.Enabled)
        {
            logger.LogInformation("Consumidor Kafka desabilitado por configuração.");
            return;
        }

        var config = new ConsumerConfig
        {
            BootstrapServers = _options.BootstrapServers,
            GroupId = _options.OrderSubmittedConsumerGroup,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false,
            ClientId = "system-distributed-orders-order-submitted-consumer"
        };

        using var consumer = new ConsumerBuilder<string, string>(config).Build();
        consumer.Subscribe(_options.OrderSubmittedTopic);

        logger.LogInformation(
            "Consumidor Kafka iniciado no tópico {Topic} com o grupo {ConsumerGroup}.",
            _options.OrderSubmittedTopic,
            _options.OrderSubmittedConsumerGroup);

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                ConsumeResult<string, string>? result;

                try
                {
                    result = consumer.Consume(stoppingToken);
                }
                catch (ConsumeException exception)
                {
                    logger.LogError(exception, "Falha ao consumir mensagem do Kafka.");
                    await DelayBeforeRetryAsync(stoppingToken);
                    continue;
                }

                if (result?.Message?.Value is null)
                    continue;

                OrderSubmittedEvent? message;

                try
                {
                    message = JsonSerializer.Deserialize<OrderSubmittedEvent>(
                        result.Message.Value,
                        SerializerOptions);
                }
                catch (JsonException exception)
                {
                    logger.LogError(
                        exception,
                        "Mensagem inválida descartada em {TopicPartitionOffset}.",
                        result.TopicPartitionOffset);
                    consumer.Commit(result);
                    continue;
                }

                if (message is null)
                {
                    logger.LogError(
                        "Mensagem vazia descartada em {TopicPartitionOffset}.",
                        result.TopicPartitionOffset);
                    consumer.Commit(result);
                    continue;
                }

                try
                {
                    await using var scope = scopeFactory.CreateAsyncScope();
                    var handler = scope.ServiceProvider
                        .GetRequiredService<IOrderSubmittedEventHandler>();

                    await handler.HandleAsync(message, stoppingToken);
                    consumer.Commit(result);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception exception)
                {
                    logger.LogError(
                        exception,
                        "Falha ao processar OrderSubmitted {EventId}. A mensagem será tentada novamente.",
                        message.EventId);

                    consumer.Seek(result.TopicPartitionOffset);
                    await DelayBeforeRetryAsync(stoppingToken);
                }
            }
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            // Encerramento normal da aplicação.
        }
        finally
        {
            consumer.Close();
        }
    }

    private static async Task DelayBeforeRetryAsync(CancellationToken cancellationToken)
    {
        await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
    }
}
