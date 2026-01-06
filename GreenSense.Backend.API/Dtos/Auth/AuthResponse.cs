namespace GreenSense.Backend.API.Dtos.Auth;

public record AuthResponse(
    int UserId,
    string Email,
    string Token
);
