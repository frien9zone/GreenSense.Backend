using GreenSense.Backend.Data.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace GreenSense.Backend.Data.Entities
{
    public class Sensor
    {
        public int SensorId { get; set; }

        public int PlantId { get; set; }
        public Plant Plant { get; set; } = null!;

        public SensorType SensorType { get; set; }

        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation
        public ICollection<SensorReading> SensorReadings { get; set; } = new List<SensorReading>();
    }
}
