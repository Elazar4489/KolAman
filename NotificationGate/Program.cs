using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using NotificationGate.Services;
using NotificationGate.Settings;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.Configure<KafkaSettings>(builder.Configuration.GetSection("Kafka"));
var path = builder.Services.Configure<PathSettings>(builder.Configuration.GetSection("PathSettings"));


builder.Services.AddSingleton<IProducer<string, string>>(sp =>
{
    var kafkaSettings = sp.GetRequiredService<IOptions<KafkaSettings>>().Value;

    var config = new ProducerConfig
    {
        BootstrapServers = kafkaSettings.BootstrapServers,
    };

    return new ProducerBuilder<string, string>(config).Build();
});


builder.Services.AddSingleton<FileWatcher>(fw =>
{
    var pathSettings = fw.GetRequiredService<IOptions<PathSettings>>();
    var kafkaService = fw.GetRequiredService<KafkaService>();
    Console.WriteLine(pathSettings.Value.Path);
    return new FileWatcher(kafkaService, pathSettings);
});


builder.Services.AddSingleton<KafkaService>();
builder.Services.AddHostedService<NotificationGateService>();

var app = builder.Build();
await app.RunAsync();