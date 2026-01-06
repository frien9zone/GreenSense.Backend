using System;
using System.Collections.Generic;
using System.Text;

namespace GreenSense.Backend.Data.Entities
{
    public class SensorReading
    {
        public int ReadingId { get; set; }

        public int SensorId { get; set; }
        public Sensor Sensor { get; set; } = null!;

        public double Value { get; set; }
        public DateTime MeasuredAt { get; set; }

        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }
}
