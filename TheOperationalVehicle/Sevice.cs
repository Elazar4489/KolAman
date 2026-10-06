using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheOperationalVehicle
{
    public class Service : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<Service> _logger;
        public Service(IServiceProvider serviceProvider, ILogger<Service> logger)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    // כאן הייתי אמור ליצור מופע של הפרוביידר של SQL ולמשוך איכשהו את הנתונים החדשים שמגיעים לטבלא ולטפל בהם.
                    // מחמת קוצר הזמן לא הספקתי לצערי.
                }
            }
            catch
            {

            }
        }
    }
}
