using GreenSense.Backend.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GreenSense.Backend.Data.Db.Configurations;

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable("notifications");

        builder.HasKey(x => x.NotificationId);

        builder.Property(x => x.PlantId)
            .IsRequired();

        builder.Property(x => x.Type)
            .IsRequired()
            .HasMaxLength(80);

        builder.Property(x => x.Message)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.IsRead)
            .IsRequired();

        builder.HasOne(x => x.Plant)
            .WithMany(p => p.Notifications)
            .HasForeignKey(x => x.PlantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Reading)
            .WithMany(r => r.Notifications)
            .HasForeignKey(x => x.ReadingId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(x => new { x.PlantId, x.CreatedAt });
    }
}
