using System.Text;
using GreenSense.Backend.Data.Db;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GreenSense.Backend.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly GreenSenseDbContext _db;

    public ReportsController(GreenSenseDbContext db)
    {
        _db = db;
    }

    // GET: api/reports/system-status
    [HttpGet("system-status")]
    public async Task<IActionResult> GetSystemStatusCsv()
    {
        // Вытягиваем строки отчёта (каждая строка = один сенсор)
        var rows = await _db.Sensors
            .AsNoTracking()
            .Select(s => new
            {
                s.PlantId,
                PlantName = s.Plant.Name, // <-- если у Plant поле называется иначе (например Title) — замени тут
                s.SensorId,
                SensorType = s.SensorType.ToString(),
                s.IsActive,

                LastReading = _db.SensorReadings
                    .AsNoTracking()
                    .Where(r => r.SensorId == s.SensorId)
                    .OrderByDescending(r => r.MeasuredAt)
                    .Select(r => new { r.Value, r.MeasuredAt })
                    .FirstOrDefault(),

                HasUnreadNotifications = _db.Notifications
                    .AsNoTracking()
                    .Any(n => n.PlantId == s.PlantId && !n.IsRead)
            })
            .OrderBy(r => r.PlantId)
            .ThenBy(r => r.SensorId)
            .ToListAsync();

        // Формируем CSV
        var sb = new StringBuilder();

        sb.AppendLine("PlantId,PlantName,SensorId,SensorType,IsActive,LastValue,LastMeasuredAt,HasUnreadNotifications");

        foreach (var r in rows)
        {
            var lastValue = r.LastReading?.Value.ToString(System.Globalization.CultureInfo.InvariantCulture) ?? "";
            var lastMeasuredAt = r.LastReading?.MeasuredAt.ToString("O") ?? ""; // ISO-8601

            sb.AppendLine(string.Join(",",
                Csv(r.PlantId.ToString()),
                Csv(r.PlantName ?? ""),
                Csv(r.SensorId.ToString()),
                Csv(r.SensorType),
                Csv(r.IsActive.ToString()),
                Csv(lastValue),
                Csv(lastMeasuredAt),
                Csv(r.HasUnreadNotifications.ToString())
            ));
        }

        var bytes = Encoding.UTF8.GetBytes(sb.ToString());

        var fileName = $"greensense-system-status-{DateTime.UtcNow:yyyyMMdd-HHmmss}.csv";
        return File(bytes, "text/csv; charset=utf-8", fileName);
    }

    private static string Csv(string value)
    {
        // CSV escaping: если есть запятая/кавычки/перенос строки — оборачиваем в кавычки и удваиваем кавычки
        if (value.Contains(',') || value.Contains('"') || value.Contains('\n') || value.Contains('\r'))
        {
            value = value.Replace("\"", "\"\"");
            return $"\"{value}\"";
        }

        return value;
    }
}
