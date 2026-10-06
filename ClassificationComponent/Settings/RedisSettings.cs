using StackExchange.Redis;

namespace ClassificationComponent.Settings
{
    public class RedisSettings
    {
        public EndPointCollection EndPoints { get; set; } = new();
        public string User { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
