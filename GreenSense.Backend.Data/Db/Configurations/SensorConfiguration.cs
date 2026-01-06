using GreenSense.Backend.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GreenSense.Backend.Data.Db.Configurations;

public class SensorConfiguration : IEntityTypeConfiguration<Sensor>
{
    public void Configure(EntityTypeBuilder<Sensor> builder)
    {
        builder.ToTable("sensors");

        builder.HasKey(x => x.SensorId);

        builder.Property(x => x.PlantId)
            .IsRequired();

        builder.Property(x => x.SensorType)
            .IsRequired();

        builder.Property(x => x.IsActive)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.HasOne(x => x.Plant)
            .WithMany(p => p.Sensors)
            .HasForeignKey(x => x.PlantId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
