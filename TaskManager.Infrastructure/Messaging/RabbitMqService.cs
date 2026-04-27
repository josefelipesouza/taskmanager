using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using TaskManager.Application.Interfaces;

namespace TaskManager.Infrastructure.Messaging;

public class RabbitMqService : IMessageService, IDisposable
{
    private readonly IConnection _connection;
    private readonly IChannel _channel;
    private const string QueueName = "task-events";

    public RabbitMqService(string hostName)
    {
        var factory = new ConnectionFactory { HostName = hostName };
        _connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
        _channel = _connection.CreateChannelAsync().GetAwaiter().GetResult();

        _channel.QueueDeclareAsync(
            queue: QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false
        ).GetAwaiter().GetResult();
    }

    public async Task PublishAsync<T>(string eventName, T message)
    {
        var payload = JsonSerializer.Serialize(new
        {
            Event = eventName,
            Data = message,
            Timestamp = DateTime.UtcNow
        });

        var body = Encoding.UTF8.GetBytes(payload);

        await _channel.BasicPublishAsync(
            exchange: string.Empty,
            routingKey: QueueName,
            body: body
        );
    }

    public void Dispose()
    {
        _channel?.CloseAsync();
        _connection?.CloseAsync();
    }
}