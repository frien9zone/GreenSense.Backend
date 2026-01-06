using System;
using System.Collections.Generic;
using System.Text;

namespace GreenSense.Backend.Data.Entities
{
    public class Notification
    {
        public int NotificationId { get; set; }

        public int PlantId { get; set; }
        public Plant Plant { get; set; } = null!;

        public int? ReadingId { get; set; }
        public SensorReading? Reading { get; set; }

        public string Type { get; set; } = null!;
        public string Message { get; set; } = null!;

        public DateTime CreatedAt { get; set; }
        public bool IsRead { get; set; }
    }
}
