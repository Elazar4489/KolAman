using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using NotificationGate.Settings;

namespace NotificationGate.Services
{
    public class NotificationGateService : BackgroundService
    {
        private readonly FileWatcher _fileWatcher;

        public NotificationGateService(FileWatcher fileWatcher)
        {
            _fileWatcher = fileWatcher;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    await _fileWatcher.Manager();
                }
            }
            catch (FileNotFoundException e)
            {
                throw new FileNotFoundException();
            }
            catch (Exception)
            {
                throw new NotImplementedException();
            }
            
        }

    }
}