using GreenSense.Backend.API.Dtos.Readings;
using GreenSense.Backend.API.Mapping;
using GreenSense.Backend.API.Services;
using GreenSense.Backend.Data.Db;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GreenSense.Backend.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SensorReadingsController : ControllerBase
{
    private readonly GreenSenseDbContext _db;
    private readonly IReadingNotificationService _readingNotificationService;

    public SensorReadingsController(GreenSenseDbContext db, IReadingNotificationService readingNotificationService)
    {
        _db = db;
        _readingNotificationService = readingNotificationService;
    }

    // POST: api/sensorreadings
    [HttpPost]
    public async Task<ActionResult<ReadingResponse>> Create([FromBody] ReadingCreateRequest request)
    {
        var sensorExists = await _db.Sensors.AsNoTracking().AnyAsync(s => s.SensorId == request.SensorId);
        if (!sensorExists)
            return BadRequest("SensorId not found.");

        var reading = request.ToEntity();

        _db.SensorReadings.Add(reading);
        await _db.SaveChangesAsync();

        await _readingNotificationService.EvaluateAndCreateNotificationIfNeededAsync(reading.ReadingId);

        return CreatedAtAction(nameof(GetById), new { id = reading.ReadingId }, reading.ToResponse());
    }

    // GET: api/sensorreadings/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ReadingResponse>> GetById([FromRoute] int id)
    {
        var reading = await _db.SensorReadings.AsNoTracking().FirstOrDefaultAsync(r => r.ReadingId == id);
        if (reading is null)
            return NotFound();

        return reading.ToResponse();
    }

    // GET: api/sensorreadings
    [HttpGet]
    public async Task<ActionResult<List<ReadingResponse>>> GetList(
        [FromQuery] int sensorId,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        [FromQuery] int limit = 200)
    {
        if (limit < 1) limit = 1;
        if (limit > 2000) limit = 2000;

        var query = _db.SensorReadings
            .AsNoTracking()
            .Where(r => r.SensorId == sensorId);

        if (from.HasValue)
            query = query.Where(r => r.MeasuredAt >= from.Value);

        if (to.HasValue)
            query = query.Where(r => r.MeasuredAt <= to.Value);

        var readings = await query
            .OrderByDescending(r => r.MeasuredAt)
            .Take(limit)
            .ToListAsync();

        return readings.Select(r => r.ToResponse()).ToList();
    }

    // DELETE: api/sensorreadings/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        var reading = await _db.SensorReadings.FirstOrDefaultAsync(r => r.ReadingId == id);
        if (reading is null)
            return NotFound();

        _db.SensorReadings.Remove(reading);
        await _db.SaveChangesAsync();

        return NoContent();
    }
}
