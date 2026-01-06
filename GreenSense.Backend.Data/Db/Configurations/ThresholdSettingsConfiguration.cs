using GreenSense.Backend.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GreenSense.Backend.Data.Db.Configurations;

public class ThresholdSettingsConfiguration : IEntityTypeConfiguration<ThresholdSettings>
{
    public void Configure(EntityTypeBuilder<ThresholdSettings> builder)
    {
        builder.ToTable("threshold_settings");

        builder.HasKey(x => x.ThresholdId);

        builder.Property(x => x.PlantId)
            .IsRequired();

        builder.Property(x => x.SoilMin).IsRequired();
        builder.Property(x => x.SoilMax).IsRequired();
        builder.Property(x => x.TempMin).IsRequired();
        builder.Property(x => x.TempMax).IsRequired();

        builder.Property(x => x.UpdatedAt)
            .IsRequired();

        builder.HasIndex(x => x.PlantId)
            .IsUnique();

        builder.HasOne(x => x.Plant)
            .WithOne(p => p.ThresholdSettings)
            .HasForeignKey<ThresholdSettings>(x => x.PlantId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
