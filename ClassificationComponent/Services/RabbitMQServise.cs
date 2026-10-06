using ClassificationComponent.Models;
using ClassificationComponent.Settings;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace ClassificationComponent.Services
{
    public class RabbitMQServise
    {
        private readonly IConnectionFactory _connectionFactory;
        private IConnection? _connection;
        private readonly SemaphoreSlim _lock = new(1, 1);
        private readonly RabbitMQSettings _rabbitMQSettings;
        public RabbitMQServise(IConnectionFactory rabbitConnectionFactory,
            IOptions<RabbitMQSettings> options)
        {
            _connectionFactory = rabbitConnectionFactory;
            _rabbitMQSettings = options.Value;
        }
        public async Task<IChannel> CreateChannelAsync()
        {
            if (_connection == null || !_connection.IsOpen)
            {
                await _lock.WaitAsync();
                try
                {
                    if (_connection == null || !_connection.IsOpen)
                    {
                        _connection = await _connectionFactory.CreateConnectionAsync();
                    }
                }
                finally
                {
                    _lock.Release();
                }
            }

            return await _connection.CreateChannelAsync();
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
        public async Task PublishingAQueueAlertTaskInRabbitMQ(Alert warning, IChannel channel, string routingKey)
        {
            var message = JsonSerializer.Serialize(warning);
            var body = Encoding.UTF8.GetBytes(message);
            await channel.BasicPublishAsync(
                exchange: _rabbitMQSettings.ExchangeName,
                routingKey: routingKey,
                body: body);
        }
    }
}
