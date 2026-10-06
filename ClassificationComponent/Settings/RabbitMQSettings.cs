using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassificationComponent.Settings
{
    public class RabbitMQSettings
    {
        public string Host { get; set; } = string.Empty;
        public int Port { get; set; }
        public string VirtualHost { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ExchangeName { get; set; } = string.Empty;
        public string ExchangeType { get; set; } = string.Empty;
        public string CentralCommand { get; set; } = string.Empty;
        public string RoutingKeyCentral { get; set; } = string.Empty;
        public string SouthernCommand { get; set; } = string.Empty;
        public string RoutingKeySouthern { get; set; } = string.Empty;
        public string NorthernCommand { get; set; } = string.Empty;
        public string RoutingKeyNorthern { get; set; } = string.Empty;
        public string DeepCommand { get; set; } = string.Empty;
        public string RoutingKeyDeep { get; set; } = string.Empty;
    }
}