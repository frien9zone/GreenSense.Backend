using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using GreenSense.Backend.Data.Db;
using GreenSense.Backend.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace GreenSense.Backend.API.Services;

public class ReadingNotificationService : IReadingNotificationService
{
    private readonly GreenSenseDbContext _db;

    public ReadingNotificationService(GreenSenseDbContext db)
    {
        _db = db;
    }

    public async Task EvaluateAndCreateNotificationIfNeededAsync(int readingId, CancellationToken ct = default)
    {
        var reading = await _db.SensorReadings
            .AsNoTracking()
            .Include(r => r.Sensor)
            .FirstOrDefaultAsync(r => r.ReadingId == readingId, ct);

        if (reading is null)
            return;

        var sensor = reading.Sensor;
        var plantId = sensor.PlantId;

        var thresholds = await _db.ThresholdSettings
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.PlantId == plantId, ct);

        if (thresholds is null)
            return;

        var sensorTypeName = sensor.SensorType.ToString();

        double min;
        double max;

        if (sensorTypeName.Contains("soil", StringComparison.OrdinalIgnoreCase))
        {
            min = thresholds.SoilMin;
            max = thresholds.SoilMax;
        }
        else if (sensorTypeName.Contains("temp", StringComparison.OrdinalIgnoreCase))
        {
            min = thresholds.TempMin;
            max = thresholds.TempMax;
        }
        else
        {
            return;
        }

        var value = reading.Value;

        var belowMin = value < min;
        var aboveMax = value > max;

        if (!belowMin && !aboveMax)
            return;

        var type = belowMin ? "BelowMin" : "AboveMax";

        var message = belowMin
            ? $"{sensorTypeName}: value {value.ToString(CultureInfo.InvariantCulture)} ниже мінімуму {min.ToString(CultureInfo.InvariantCulture)}"
            : $"{sensorTypeName}: value {value.ToString(CultureInfo.InvariantCulture)} вище максимуму {max.ToString(CultureInfo.InvariantCulture)}";


        var notification = new Notification
        {
            PlantId = plantId,
            ReadingId = reading.ReadingId,
            Type = type,
            Message = message,
            CreatedAt = DateTime.UtcNow,
            IsRead = false
        };

        _db.Notifications.Add(notification);
        await _db.SaveChangesAsync(ct);
    }
}
