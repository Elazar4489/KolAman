using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace CommandSystemDB.Settings
{
    public class CommandSystemDbContextFactory : IDesignTimeDbContextFactory<CommandSystemDbContext>
    {
        public CommandSystemDbContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .Build();

            var builder = new DbContextOptionsBuilder<CommandSystemDbContext>();

            var connectionString = configuration.GetConnectionString("MySQL");

            builder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

            return new CommandSystemDbContext(builder.Options);
        }
    }
}