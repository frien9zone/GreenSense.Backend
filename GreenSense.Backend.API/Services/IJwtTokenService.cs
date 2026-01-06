using GreenSense.Backend.Data.Entities;

namespace GreenSense.Backend.API.Services;

public interface IJwtTokenService
{
    string CreateToken(User user);
}
