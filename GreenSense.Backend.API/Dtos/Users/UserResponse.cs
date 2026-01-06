namespace GreenSense.Backend.API.Dtos.Users;

public record UserResponse(
    int UserId,
    string Email,
    DateTime CreatedAt
);
