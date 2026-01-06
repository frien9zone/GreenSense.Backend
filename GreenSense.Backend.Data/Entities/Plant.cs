using System;
using System.Collections.Generic;
using System.Text;

namespace GreenSense.Backend.Data.Entities
{
    public class Plant
    {
        public int PlantId { get; set; }

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public string Name { get; set; } = null!;
        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; }

        public ICollection<Sensor> Sensors { get; set; } = new List<Sensor>();
        public ThresholdSettings? ThresholdSettings { get; set; }
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }
}
