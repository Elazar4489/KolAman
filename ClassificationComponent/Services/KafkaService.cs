using ClassificationComponent.Models;
using ClassificationComponent.Settings;
using Confluent.Kafka;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static Confluent.Kafka.ConfigPropertyNames;

namespace ClassificationComponent.Services
{
    public class KafkaService
    {
        private static readonly JsonSerializerOptions jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };
        private readonly IConsumer<string, string> _consumer;
        private readonly KafkaSettings _kafkaSettings;
        public KafkaService(IConsumer<string, string> consumer,
            IOptions<KafkaSettings> kafkaSettings)
        {
            _consumer = consumer;
            _kafkaSettings = kafkaSettings.Value;
        }
        public Alert? GetConsumer(CancellationToken stoppingToken)
        {
            _consumer.Subscribe(_kafkaSettings.Topic);
            var consumeResult = _consumer.Consume(stoppingToken);
            if (consumeResult?.Message?.Value == null)
            {
                return null;
            }
            Alert? alert = null;
            try
            {
                alert = JsonSerializer.Deserialize<Alert>(consumeResult.Message.Value, jsonOptions);
                return alert;
            }
            catch (JsonException)
            {
                _consumer.Commit(consumeResult);
                return null;
            }
        }
        public async Task ConsumerCommit()
        {
            _consumer.Commit();
        }
        public async Task ConsumerClose()
        {
            _consumer.Close();
        }


    }
}
