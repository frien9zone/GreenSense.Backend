namespace GreenSense.Backend.API.Dtos.Auth;

public record RegisterRequest(
    string Email,
    string Password
);
