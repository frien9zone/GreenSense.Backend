using GreenSense.Backend.API.Dtos.Plants;
using GreenSense.Backend.API.Mapping;
using GreenSense.Backend.Data.Db;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GreenSense.Backend.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlantsController : ControllerBase
    {
        private readonly GreenSenseDbContext _db;

        public PlantsController(GreenSenseDbContext db)
        {
            _db = db;
        }

        // POST: api/plants
        [HttpPost]
        public async Task<ActionResult<PlantResponse>> Create([FromBody] PlantCreateRequest request)
        {
            // optional: проверим что user существует
            var userExists = await _db.Users.AnyAsync(u => u.UserId == request.UserId);
            if (!userExists)
                return BadRequest("UserId not found.");

            var plant = request.ToEntity();

            _db.Plants.Add(plant);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = plant.PlantId }, plant.ToResponse());
        }

        // GET: api/plants?userId=1
        [HttpGet]
        public async Task<ActionResult<List<PlantResponse>>> GetAll([FromQuery] int userId)
        {
            var plants = await _db.Plants
                .AsNoTracking()
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            return plants.Select(p => p.ToResponse()).ToList();
        }

        // GET: api/plants/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<PlantResponse>> GetById([FromRoute] int id)
        {
            var plant = await _db.Plants
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.PlantId == id);

            if (plant is null)
                return NotFound();

            return plant.ToResponse();
        }

        // PUT: api/plants/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] PlantUpdateRequest request)
        {
            var plant = await _db.Plants.FirstOrDefaultAsync(p => p.PlantId == id);
            if (plant is null)
                return NotFound();

            plant.ApplyUpdate(request);
            await _db.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/plants/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var plant = await _db.Plants.FirstOrDefaultAsync(p => p.PlantId == id);
            if (plant is null)
                return NotFound();

            _db.Plants.Remove(plant);
            await _db.SaveChangesAsync();

            return NoContent();
        }
    }
}
