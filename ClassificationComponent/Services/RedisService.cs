using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace ClassificationComponent.Services
{
    public class RedisService
    {
        private readonly IDatabase _database;
        private readonly ILogger<RedisService> _logger;
        private readonly TimeSpan _ttl = TimeSpan.FromHours(2);
        public RedisService(IConnectionMultiplexer connection, ILogger<RedisService> logger)
        {
            _database = connection.GetDatabase();
            _logger = logger;
        }
        public async Task<bool> IsDuplicateOrAddAsync(string alertId)
        {
            string redisKey = $"alert:{alertId}";
            bool isNew = await _database.StringSetAsync(redisKey, "exists", _ttl, When.NotExists);
            if (!isNew)
            {
                _logger.LogWarning("alert {alertId} already exists in redis", alertId);
                return true;
            }
            _logger.LogInformation("");
            return false;
        }
    }
}
