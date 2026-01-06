using GreenSense.Backend.API.Dtos.Thresholds;
using GreenSense.Backend.API.Mapping;
using GreenSense.Backend.Data.Db;
using GreenSense.Backend.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GreenSense.Backend.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ThresholdSettingsController : ControllerBase
{
    private readonly GreenSenseDbContext _db;

    public ThresholdSettingsController(GreenSenseDbContext db)
    {
        _db = db;
    }

    // GET: api/thresholdsettings?plantId=123
    [HttpGet]
    public async Task<ActionResult<ThresholdResponse>> Get([FromQuery] int plantId)
    {
        var threshold = await _db.ThresholdSettings
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.PlantId == plantId);

        if (threshold is null)
            return NotFound("Threshold settings not found.");

        return threshold.ToResponse();
    }

    // PUT: api/thresholdsettings?plantId=123
    // Upsert
    [HttpPut]
    public async Task<ActionResult<ThresholdResponse>> Upsert(
        [FromQuery] int plantId,
        [FromBody] ThresholdUpdateRequest request)
    {
        var plantExists = await _db.Plants.AnyAsync(p => p.PlantId == plantId);
        if (!plantExists)
            return BadRequest("PlantId not found.");

        var threshold = await _db.ThresholdSettings.FirstOrDefaultAsync(t => t.PlantId == plantId);

        if (threshold is null)
        {
            threshold = new ThresholdSettings
            {
                PlantId = plantId,
                UpdatedAt = DateTime.UtcNow
            };

            threshold.ApplyUpdate(request);

            _db.ThresholdSettings.Add(threshold);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { plantId }, threshold.ToResponse());
        }

        threshold.ApplyUpdate(request);
        await _db.SaveChangesAsync();

        return Ok(threshold.ToResponse());
    }

    // DELETE: api/thresholdsettings?plantId=123
    [HttpDelete]
    public async Task<IActionResult> Delete([FromQuery] int plantId)
    {
        var threshold = await _db.ThresholdSettings.FirstOrDefaultAsync(t => t.PlantId == plantId);
        if (threshold is null)
            return NotFound("Threshold settings not found.");

        _db.ThresholdSettings.Remove(threshold);
        await _db.SaveChangesAsync();

        return NoContent();
    }
}
