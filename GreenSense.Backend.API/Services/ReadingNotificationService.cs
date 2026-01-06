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
        // Берём reading + sensor (чтобы получить PlantId и SensorType)
        var reading = await _db.SensorReadings
            .AsNoTracking()
            .Include(r => r.Sensor)
            .FirstOrDefaultAsync(r => r.ReadingId == readingId, ct);

        if (reading is null)
            return;

        var sensor = reading.Sensor;
        var plantId = sensor.PlantId;

        // Берём последние threshold settings для plant
        var thresholds = await _db.ThresholdSettings
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.PlantId == plantId, ct);

        if (thresholds is null)
            return;

        // Определяем, какие Min/Max использовать по SensorType
        // (чтобы не зависеть от точных названий enum)
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
            // Для других типов сенсоров (если появятся) пороги пока не настроены
            return;
        }

        var value = reading.Value;

        var belowMin = value < min;
        var aboveMax = value > max;

        if (!belowMin && !aboveMax)
            return;

        // Тип нотификации
        var type = belowMin ? "BelowMin" : "AboveMax";

        // Сообщение (простое, но понятное)
        var message = belowMin
            ? $"{sensorTypeName}: value {value.ToString(CultureInfo.InvariantCulture)} ниже мінімуму {min.ToString(CultureInfo.InvariantCulture)}"
            : $"{sensorTypeName}: value {value.ToString(CultureInfo.InvariantCulture)} вище максимуму {max.ToString(CultureInfo.InvariantCulture)}";

        // (Опционально) анти-спам: не создавать дубликат такого же типа за последние N минут
        // Сейчас пропускаем, чтобы было проще по лабе.

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
