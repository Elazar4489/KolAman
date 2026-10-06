using CommandSystemDB.Enums;
using System.ComponentModel.DataAnnotations;

namespace TheOperationalVehicle.Models
{
    public class Alert
    {
        [Required]
        public string alert_id { get; set; } = string.Empty;
        [Required]
        public string source { get; set; } = string.Empty;
        public string command { get; set; } = string.Empty;
        public string title { get; set; } = string.Empty;
        public string content { get; set; } = string.Empty;
        [EnumDataType(typeof(PriorityEnum))]
        public string priority { get; set; } = string.Empty; //CRITICAL > HIGH > MEDIUM > LOW
        [EnumDataType(typeof(ClassificationEnum))]
        public string classification { get; set; } = string.Empty;//UNCLASSIFIED / RESTRICTED / SECRET / TOP_SECRET
        [Required]
        [Range(-90, 90)]
        public double lat { get; set; }
        [Required]
        [Range(-180, 180)]
        public double lon { get; set; }
        [Required]
        public DateTime timestamp { get; set; }
        public string status { get; set; } = "WAITING";

    }
}
