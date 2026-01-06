using System.Security.Claims;

namespace GreenSense.Backend.API.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static int GetUserId(this ClaimsPrincipal user)
    {
        var id = user.FindFirstValue("userId") ?? user.FindFirstValue(ClaimTypes.NameIdentifier);
        if (id is null || !int.TryParse(id, out var userId))
            throw new InvalidOperationException("UserId claim is missing.");

        return userId;
    }
}
