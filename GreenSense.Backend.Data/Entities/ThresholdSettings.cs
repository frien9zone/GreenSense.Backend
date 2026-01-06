using System;
using System.Collections.Generic;
using System.Text;

namespace GreenSense.Backend.Data.Entities
{
    public class ThresholdSettings
    {
        public int ThresholdId { get; set; }

        public int PlantId { get; set; }
        public Plant Plant { get; set; } = null!;

        public double SoilMin { get; set; }
        public double SoilMax { get; set; }

        public double TempMin { get; set; }
        public double TempMax { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
