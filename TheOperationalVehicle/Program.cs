
using Elastic.Clients.Elasticsearch;
using Elastic.Transport;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Serilog;

var builder = Host.CreateApplicationBuilder(args);
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();



//var connectionStringMysql = builder.Configuration.GetConnectionString("MySql");
//builder.Services.AddDbContext<CommandSystemDbContext>(options =>
//    options.UseMySql(connectionStringMysql, ServerVersion.AutoDetect(connectionStringMysql)));