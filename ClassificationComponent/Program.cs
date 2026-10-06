using ClassificationComponent.Services;
using ClassificationComponent.Settings;
using Confluent.Kafka;
using Elastic.Clients.Elasticsearch;
using Elastic.Transport;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using Serilog;
using StackExchange.Redis;




var builder = Host.CreateApplicationBuilder(args);
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Services.Configure<KafkaSettings>(builder.Configuration.GetSection("Kafka"));
builder.Services.Configure<RedisSettings>(builder.Configuration.GetSection("Redis"));
builder.Services.Configure<RabbitMQSettings>(builder.Configuration.GetSection("RabbitMQ"));
builder.Services.Configure<ElasticSettings>(builder.Configuration.GetSection("Elasticsearch"));

builder.Services.AddSingleton<IConsumer<string, string>>(sp =>
{
    var kafkaSettings = sp.GetRequiredService<IOptions<KafkaSettings>>().Value;

    var config = new ConsumerConfig
    {
        BootstrapServers = "localhost:9094",//kafkaSettings.BootstrapServers,
        GroupId = "kafkaSettings.GroupId",
        AutoOffsetReset = Enum.Parse<AutoOffsetReset>("Earliest", ignoreCase: true),//kafkaSettings.AutoOffsetReset
        EnableAutoCommit = true //kafkaSettings.EnableAutoCommit
    };

    return new ConsumerBuilder<string, string>(config).Build();
});
builder.Services.AddSingleton<IConnectionMultiplexer>(cm =>
{
    var redisSrttings = cm.GetRequiredService<IOptions<RedisSettings>>().Value;
    ConfigurationOptions conf = new ConfigurationOptions
    {
        EndPoints = { "localhost:6379" }, //redisSrttings.EndPoints,
        User = redisSrttings.User,
        Password = redisSrttings.Password,
        AbortOnConnectFail = false
    };
    return ConnectionMultiplexer.Connect(conf);
});
builder.Services.AddSingleton(sp =>
{
    var elasticSettings = sp.GetRequiredService<IOptions<ElasticSettings>>().Value;

    var settings = new ElasticsearchClientSettings(new Uri(elasticSettings.Node))
        .DefaultIndex(elasticSettings.DefaultIndex);

    if (!string.IsNullOrEmpty(elasticSettings.Username) && !string.IsNullOrEmpty(elasticSettings.Password))
    {
        settings.Authentication(new BasicAuthentication(elasticSettings.Username, elasticSettings.Password));
    }

    return new ElasticsearchClient(settings);
});
builder.Services.AddSingleton<IConnectionFactory>(sp =>
{
    var rabbitSettings = sp.GetRequiredService<IOptions<RabbitMQSettings>>().Value;
    return new ConnectionFactory
    {
        HostName = rabbitSettings.Host,
        Port = rabbitSettings.Port,
        VirtualHost = rabbitSettings.VirtualHost,
        UserName = rabbitSettings.Username,
        Password = rabbitSettings.Password
    };
});
builder.Services.AddSingleton(new GeographicClassificationService(@"C:\Users\elazar\KolAman\alert-simulator\regions.geojson"));
builder.Services.AddSingleton<KafkaService>();
builder.Services.AddSingleton<RedisService>();
builder.Services.AddSingleton<RabbitMQServise>();
builder.Services.AddSingleton<ValidatorE>();

builder.Services.AddHostedService<ClassificationComponentWorker>();
var app = builder.Build();
await app.RunAsync();
