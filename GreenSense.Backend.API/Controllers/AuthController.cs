using GreenSense.Backend.API.Dtos.Auth;
using GreenSense.Backend.API.Security;
using GreenSense.Backend.API.Services;
using GreenSense.Backend.Data.Db;
using GreenSense.Backend.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GreenSense.Backend.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly GreenSenseDbContext _db;
    private readonly IJwtTokenService _jwt;

    public AuthController(GreenSenseDbContext db, IJwtTokenService jwt)
    {
        _db = db;
        _jwt = jwt;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        var exists = await _db.Users.AnyAsync(u => u.Email.ToLower() == email);
        if (exists)
            return Conflict("Email is already registered.");

        var user = new User
        {
            Email = email,
            PasswordHash = PasswordHasher.Hash(request.Password),
            CreatedAt = DateTime.UtcNow
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        var token = _jwt.CreateToken(user);
        return Ok(new AuthResponse(user.UserId, user.Email, token));
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == email);
        if (user is null)
            return Unauthorized("Invalid email or password.");

        var ok = PasswordHasher.Verify(request.Password, user.PasswordHash);
        if (!ok)
            return Unauthorized("Invalid email or password.");

        var token = _jwt.CreateToken(user);
        return Ok(new AuthResponse(user.UserId, user.Email, token));
    }
}
