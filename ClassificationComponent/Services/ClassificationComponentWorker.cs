using ClassificationComponent.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ClassificationComponent.Services
{
    public class ClassificationComponentWorker : BackgroundService
    {
        private readonly KafkaService _kafkaService;
        private readonly RabbitMQServise _rabbitMQServise;
        private readonly RedisService _redisService;
        private readonly GeographicClassificationService _geographic;
        private readonly ValidatorE _validator;
        private readonly ILogger<ClassificationComponentWorker> _logger;

        public ClassificationComponentWorker(
            KafkaService kafkaService,
            RabbitMQServise rabbitMQServise,
            RedisService redisService,
            GeographicClassificationService geographic,
            ValidatorE validator,
            ILogger<ClassificationComponentWorker> logger)
        {
            _kafkaService = kafkaService;
            _rabbitMQServise = rabbitMQServise;
            _redisService = redisService;
            _geographic = geographic;
            _validator = validator;
            _logger = logger;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var channel = await _rabbitMQServise.CreateChannelAsync();
            await _rabbitMQServise.DeclareAsync(channel);
            string? alertId = null;
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    Alert? alert = _kafkaService.GetConsumer(stoppingToken);
                    Console.WriteLine(alert?.alert_id ?? "null");
                    if (alert == null) continue;
                    if (alert.alert_id == null) continue;
                    else
                    {
                        alertId = alert.alert_id;
                        if (await _redisService.IsDuplicateOrAddAsync(alert.alert_id))
                        {
                            continue;
                        }
                        if (!_validator.IsValid(alert))
                        {
                            continue;
                        }
                        string routingKey = _geographic.ClassifyRegion(alert.lat, alert.lon);
                        await _rabbitMQServise.PublishingAQueueAlertTaskInRabbitMQ(alert, channel, routingKey);
                        
                        
                    }
                    await _kafkaService.ConsumerCommit();
                }
            }
            catch (RabbitMQ.Client.Exceptions.BrokerUnreachableException rmqEx)
            {
                _logger.LogError(rmqEx, "Could not reach RabbitMQ broker while publishing critical alert for alert Id: {alert_id}", alertId);
            }
            catch (OperationCanceledException)
            {
                _logger.LogError("An asynchronous or long-running operation was canceled before it could complete successfully.");
            }
            catch (Exception rmqGeneralEx)
            {
                _logger.LogError(rmqGeneralEx.Message, "Unexpected error publishing to RabbitMQ for alert Id: {alert_id}", alertId);
            }
            finally
            {
                await _kafkaService.ConsumerClose();
            }
        }
    }
}