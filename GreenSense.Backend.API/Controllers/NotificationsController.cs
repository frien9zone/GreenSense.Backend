using GreenSense.Backend.API.Dtos.Notifications;
using GreenSense.Backend.API.Mapping;
using GreenSense.Backend.Data.Db;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GreenSense.Backend.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificationsController : ControllerBase
{
    private readonly GreenSenseDbContext _db;

    public NotificationsController(GreenSenseDbContext db)
    {
        _db = db;
    }

    // GET: api/notifications?plantId=123&onlyUnread=true&limit=100
    [HttpGet]
    public async Task<ActionResult<List<NotificationResponse>>> GetList(
        [FromQuery] int? plantId,
        [FromQuery] bool onlyUnread = false,
        [FromQuery] int limit = 100)
    {
        if (limit < 1) limit = 1;
        if (limit > 500) limit = 500;

        var query = _db.Notifications.AsNoTracking().AsQueryable();

        if (plantId.HasValue)
            query = query.Where(n => n.PlantId == plantId.Value);

        if (onlyUnread)
            query = query.Where(n => !n.IsRead);

        var notifications = await query
            .OrderByDescending(n => n.CreatedAt)
            .Take(limit)
            .ToListAsync();

        return notifications.Select(n => n.ToResponse()).ToList();
    }

    // GET: api/notifications/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<NotificationResponse>> GetById([FromRoute] int id)
    {
        var notification = await _db.Notifications.AsNoTracking().FirstOrDefaultAsync(n => n.NotificationId == id);
        if (notification is null)
            return NotFound();

        return notification.ToResponse();
    }

    // PUT: api/notifications/5/read
    [HttpPut("{id:int}/read")]
    public async Task<IActionResult> MarkRead([FromRoute] int id, [FromBody] NotificationMarkReadRequest request)
    {
        var notification = await _db.Notifications.FirstOrDefaultAsync(n => n.NotificationId == id);
        if (notification is null)
            return NotFound();

        notification.IsRead = request.IsRead;
        await _db.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/notifications/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        var notification = await _db.Notifications.FirstOrDefaultAsync(n => n.NotificationId == id);
        if (notification is null)
            return NotFound();

        _db.Notifications.Remove(notification);
        await _db.SaveChangesAsync();

        return NoContent();
    }
}
