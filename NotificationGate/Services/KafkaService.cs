using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NotificationGate.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationGate.Services
{
    public class KafkaService
    {
        private readonly IProducer<string, string> _producer;
        private readonly KafkaSettings _kafkaSettings;
        private readonly ILogger<KafkaService> _logger;

        public KafkaService(IProducer<string, string> producer, IOptions<KafkaSettings> options, ILogger<KafkaService> logger)
        {
            _producer = producer;
            _kafkaSettings = options.Value;
            _logger = logger;
        }
        public async Task SendToKafka(Message<string, string> message)
        {
            _producer.Produce(_kafkaSettings.Topic, message);
            _logger.LogInformation("");
        }
    }
}
