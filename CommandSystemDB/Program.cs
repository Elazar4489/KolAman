using CommandSystemDB.Services;
using CommandSystemDB.Settings;
using Elastic.Clients.Elasticsearch;
using Elastic.Transport;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using Serilog;

var builder = Host.CreateApplicationBuilder(args);
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Services.Configure<RabbitMQSettings>(builder.Configuration.GetSection("RabbitMQ"));


var connectionStringMysql = builder.Configuration.GetConnectionString("MySql");
builder.Services.AddDbContext<CommandSystemDbContext>(options =>
    options.UseMySql(connectionStringMysql, ServerVersion.AutoDetect(connectionStringMysql)));

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
builder.Services.AddHostedService<CommandSystemWorker>();
var app = builder.Build();
//using (var scope = app.Services.CreateScope())
//{
//    var dbContext = scope.ServiceProvider.GetRequiredService<CommandSystemDbContext>();
//    dbContext.Database.Migrate();
//}


await app.RunAsync();


