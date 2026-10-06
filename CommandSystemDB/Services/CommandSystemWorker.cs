using CommandSystemDB.Models;
using CommandSystemDB.Settings;
using Elastic.Clients.Elasticsearch;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace CommandSystemDB.Services
{
    public class CommandSystemWorker : BackgroundService
    {
        private readonly string[] _queuesToListen = {"alert.central.command","alert.southern.command","alert.northern.command", "alert.deep.command"};
        private readonly IConnectionFactory _connectionFactory;
        private readonly RabbitMQSettings _rabbitMQSettings;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<CommandSystemWorker> _logger;
        private IConnection? _connection;
        private IChannel? _channel;

        public CommandSystemWorker(
            IOptions<RabbitMQSettings> options,
            IConnectionFactory connectionFactory,
            IServiceProvider serviceProvider,
            ILogger<CommandSystemWorker> logger)
        {
            _rabbitMQSettings = options.Value;
            _connectionFactory = connectionFactory;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _connection = await _connectionFactory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();
            await _channel.BasicQosAsync(
                prefetchSize: 0,
                prefetchCount: 1,
                global: false,
                cancellationToken: stoppingToken
                );
            await DeclareAsync(_channel);
            foreach (var queueName in _queuesToListen)
            {
                var consumer = new AsyncEventingBasicConsumer(_channel);

                consumer.ReceivedAsync += async (model, ea) =>
                {
                    try
                    {
                        var body = ea.Body.ToArray();
                        var messageJson = Encoding.UTF8.GetString(body);

                        Alert? alertMessage = JsonSerializer.Deserialize<Alert>(messageJson);
                        if (alertMessage != null)
                        {
                            alertMessage.command = ea.RoutingKey;
                            using var scope = _serviceProvider.CreateScope();
                            var dbContext = scope.ServiceProvider.GetRequiredService<CommandSystemDbContext>();
                            dbContext.alerts.Add(alertMessage);
                            await dbContext.SaveChangesAsync(stoppingToken);

                            await _channel.BasicAckAsync(ea.DeliveryTag, multiple: false);
                            _logger.LogInformation("Successfully processed critical alert for alert ID: {AlertId}", alertMessage.alert_id);
                        }
                        else
                        {
                            _logger.LogWarning("Anomaly with ID {AlertId} not found in database.", alertMessage?.alert_id ?? "null");
                            await _channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: false);
                        }

                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error processing critical alert message. Sending to DLQ.");
                        await _channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: false);
                    }
                };

                // Start consuming from the current queueName
                await _channel.BasicConsumeAsync(
                    queue: queueName,
                    autoAck: false,
                    consumer: consumer,
                    cancellationToken: stoppingToken);
            }
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }
        public async Task DeclareAsync(IChannel channel)
        {
            await channel.ExchangeDeclareAsync(
            exchange: _rabbitMQSettings.ExchangeName,
            type: _rabbitMQSettings.ExchangeType,
            durable: true,
            autoDelete: false,
            arguments: null);

            await channel.QueueDeclareAsync(
            queue: _rabbitMQSettings.CentralCommand,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);
            await channel.QueueBindAsync(
            queue: _rabbitMQSettings.CentralCommand,
            exchange: _rabbitMQSettings.ExchangeName,
            routingKey: _rabbitMQSettings.RoutingKeyCentral);

            await channel.QueueDeclareAsync(
            queue: _rabbitMQSettings.DeepCommand,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);
            await channel.QueueBindAsync(
            queue: _rabbitMQSettings.DeepCommand,
            exchange: _rabbitMQSettings.ExchangeName,
            routingKey: _rabbitMQSettings.RoutingKeyDeep);

            await channel.QueueDeclareAsync(
            queue: _rabbitMQSettings.NorthernCommand,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);
            await channel.QueueBindAsync(
            queue: _rabbitMQSettings.NorthernCommand,
            exchange: _rabbitMQSettings.ExchangeName,
            routingKey: _rabbitMQSettings.RoutingKeyNorthern);

            await channel.QueueDeclareAsync(
            queue: _rabbitMQSettings.SouthernCommand,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);
            await channel.QueueBindAsync(
            queue: _rabbitMQSettings.SouthernCommand,
            exchange: _rabbitMQSettings.ExchangeName,
            routingKey: _rabbitMQSettings.RoutingKeySouthern);
        }
    }
}