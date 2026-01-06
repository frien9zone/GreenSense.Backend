using GreenSense.Backend.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace GreenSense.Backend.Data.Db;

public class GreenSenseDbContext : DbContext
{
    public GreenSenseDbContext(DbContextOptions<GreenSenseDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Plant> Plants => Set<Plant>();
    public DbSet<Sensor> Sensors => Set<Sensor>();
    public DbSet<SensorReading> SensorReadings => Set<SensorReading>();
    public DbSet<ThresholdSettings> ThresholdSettings => Set<ThresholdSettings>();
    public DbSet<Notification> Notifications => Set<Notification>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(GreenSenseDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
