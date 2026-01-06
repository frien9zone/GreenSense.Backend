using GreenSense.Backend.API.Dtos.Sensors;
using GreenSense.Backend.API.Mapping;
using GreenSense.Backend.Data.Db;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GreenSense.Backend.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SensorsController : ControllerBase
{
    private readonly GreenSenseDbContext _db;

    public SensorsController(GreenSenseDbContext db)
    {
        _db = db;
    }

    // POST: api/sensors
    [HttpPost]
    public async Task<ActionResult<SensorResponse>> Create([FromBody] SensorCreateRequest request)
    {
        var plantExists = await _db.Plants.AsNoTracking().AnyAsync(p => p.PlantId == request.PlantId);
        if (!plantExists)
            return BadRequest("PlantId not found.");

        var sensor = request.ToEntity();

        _db.Sensors.Add(sensor);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = sensor.SensorId }, sensor.ToResponse());
    }

    // GET: api/sensors?plantId=123
    [HttpGet]
    public async Task<ActionResult<List<SensorResponse>>> GetAll([FromQuery] int plantId)
    {
        var sensors = await _db.Sensors
            .AsNoTracking()
            .Where(s => s.PlantId == plantId)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();

        return sensors.Select(s => s.ToResponse()).ToList();
    }

    // GET: api/sensors/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<SensorResponse>> GetById([FromRoute] int id)
    {
        var sensor = await _db.Sensors.AsNoTracking().FirstOrDefaultAsync(s => s.SensorId == id);
        if (sensor is null)
            return NotFound();

        return sensor.ToResponse();
    }

    // PUT: api/sensors/5
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] SensorUpdateRequest request)
    {
        var sensor = await _db.Sensors.FirstOrDefaultAsync(s => s.SensorId == id);
        if (sensor is null)
            return NotFound();

        sensor.ApplyUpdate(request);
        await _db.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/sensors/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        var sensor = await _db.Sensors.FirstOrDefaultAsync(s => s.SensorId == id);
        if (sensor is null)
            return NotFound();

        _db.Sensors.Remove(sensor);
        await _db.SaveChangesAsync();

        return NoContent();
    }
}
