namespace GreenSense.Backend.API.Dtos.Auth;

public record LoginRequest(
    string Email,
    string Password
);
