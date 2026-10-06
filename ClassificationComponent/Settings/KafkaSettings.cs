using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassificationComponent.Settings
{
    public class KafkaSettings
    {
        public string BootstrapServers { get; set; } = string.Empty;
        public string GroupId { get; set; } = string.Empty;
        public string AutoOffsetReset { get; set; } = "Earliest";
        public bool EnableAutoCommit { get; set; } = false;
        public string Topic { get; set; } = string.Empty;
    }
}
