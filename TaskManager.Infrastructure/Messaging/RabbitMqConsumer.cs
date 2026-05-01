using System.Text;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using TaskManager.Application.Interfaces;

namespace TaskManager.Infrastructure.Messaging;

public class RabbitMqConsumer : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<RabbitMqConsumer> _logger;
    private readonly string _hostName;
    private readonly string _emailTo;
    private IConnection? _connection;
    private IChannel? _channel;
    private const string QueueName = "task-events";

    public RabbitMqConsumer(
        IServiceProvider serviceProvider,
        ILogger<RabbitMqConsumer> logger,
        string hostName,
        string emailTo)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _hostName = hostName;
        _emailTo = emailTo;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory { HostName = _hostName };

        _connection = await factory.CreateConnectionAsync(stoppingToken);
        _channel = await _connection.CreateChannelAsync(cancellationToken: stoppingToken);

        await _channel.QueueDeclareAsync(
            queue: QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: stoppingToken
        );

        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.ReceivedAsync += async (_, ea) =>
        {
            try
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var payload = JsonSerializer.Deserialize<TaskEventPayload>(message);

                if (payload is not null)
                {
                    _logger.LogInformation("Evento recebido: {Event}", payload.Event);

                    using var scope = _serviceProvider.CreateScope();
                    var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

                    var subject = GetSubject(payload.Event);
                    var emailBody = GetEmailBody(payload);

                    await emailService.SendAsync(_emailTo, subject, emailBody);
                    _logger.LogInformation("E-mail enviado para {Email}", _emailTo);
                }

                await _channel.BasicAckAsync(ea.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar mensagem");
                await _channel.BasicNackAsync(ea.DeliveryTag, false, true);
            }
        };

        await _channel.BasicConsumeAsync(
            queue: QueueName,
            autoAck: false,
            consumer: consumer,
            cancellationToken: stoppingToken
        );

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    private string GetSubject(string eventName) => eventName switch
    {
        "task.created"   => "✅ Nova tarefa criada — TaskManager",
        "task.completed" => "🎉 Tarefa concluída — TaskManager",
        "task.cancelled" => "❌ Tarefa cancelada — TaskManager",
        _                => "📋 Evento TaskManager"
    };

    private string GetEmailBody(TaskEventPayload payload)
    {
        var dataJson = payload.Data.ToString() ?? "{}";
        JsonElement data;
        try
        {
            data = JsonSerializer.Deserialize<JsonElement>(dataJson);
        }
        catch
        {
            data = default;
        }

        string GetField(string field)
        {
            try
            {
                foreach (var prop in data.EnumerateObject())
                {
                    if (prop.Name.Equals(field, StringComparison.OrdinalIgnoreCase))
                        return prop.Value.ToString();
                }
                return "—";
            }
            catch { return "—"; }
        }

        return $"""
            <html>
            <body style="font-family: Arial, sans-serif; padding: 20px; background-color: #f3f4f6;">
                <div style="max-width: 600px; margin: auto; background: white; border-radius: 8px; padding: 30px; box-shadow: 0 2px 8px rgba(0,0,0,0.1);">
                    <h2 style="color: #2563eb;">📋 TaskManager — Notificação</h2>
                    <hr/>
                    <p><strong>Evento:</strong> {payload.Event}</p>
                    <p><strong>Data/Hora:</strong> {payload.Timestamp:dd/MM/yyyy HH:mm:ss}</p>
                    <hr/>
                    <h3 style="color: #374151;">Detalhes da Tarefa</h3>
                    <table style="width:100%; border-collapse: collapse;">
                        <tr style="background:#f9fafb;">
                            <td style="padding:8px; border:1px solid #e5e7eb;"><strong>ID</strong></td>
                            <td style="padding:8px; border:1px solid #e5e7eb;">{GetField("Id")}</td>
                        </tr>
                        <tr>
                            <td style="padding:8px; border:1px solid #e5e7eb;"><strong>Título</strong></td>
                            <td style="padding:8px; border:1px solid #e5e7eb;">{GetField("Title")}</td>
                        </tr>
                        <tr style="background:#f9fafb;">
                            <td style="padding:8px; border:1px solid #e5e7eb;"><strong>Status</strong></td>
                            <td style="padding:8px; border:1px solid #e5e7eb;">{GetField("Status")}</td>
                        </tr>
                        <tr>
                            <td style="padding:8px; border:1px solid #e5e7eb;"><strong>Criado em</strong></td>
                            <td style="padding:8px; border:1px solid #e5e7eb;">{GetField("CreatedAt")}</td>
                        </tr>
                        <tr>
                            <td style="padding:8px; border:1px solid #e5e7eb;"><strong>Descrição</strong></td>
                            <td style="padding:8px; border:1px solid #e5e7eb;">{GetField("Description")}</td>
                        </tr>
                    </table>
                    <hr/>
                    <p style="color: #6b7280; font-size: 12px;">
                        Este é um e-mail automático do sistema TaskManager.
                    </p>
                </div>
            </body>
            </html>
            """;
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_channel is not null)
            await _channel.CloseAsync(cancellationToken);
        if (_connection is not null)
            await _connection.CloseAsync(cancellationToken);
        await base.StopAsync(cancellationToken);
    }
}

public record TaskEventPayload(string Event, JsonElement Data, DateTime Timestamp);