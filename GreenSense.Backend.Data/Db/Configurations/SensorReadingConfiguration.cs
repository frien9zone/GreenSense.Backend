using GreenSense.Backend.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GreenSense.Backend.Data.Db.Configurations;

public class SensorReadingConfiguration : IEntityTypeConfiguration<SensorReading>
{
    public void Configure(EntityTypeBuilder<SensorReading> builder)
    {
        builder.ToTable("sensor_readings");

        builder.HasKey(x => x.ReadingId);

        builder.Property(x => x.SensorId)
            .IsRequired();

        builder.Property(x => x.Value)
            .IsRequired();

        builder.Property(x => x.MeasuredAt)
            .IsRequired();

        builder.HasOne(x => x.Sensor)
            .WithMany(s => s.SensorReadings)
            .HasForeignKey(x => x.SensorId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new { x.SensorId, x.MeasuredAt });
    }
}
